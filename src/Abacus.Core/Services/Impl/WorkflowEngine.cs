using System.Linq.Expressions;
using System.Text.Json;
using Abacus.Core.Model;
using Microsoft.Extensions.Logging;

namespace Abacus.Core.Services.Impl
{
    public class WorkflowEngine : IWorkflowEngine
    {
        private readonly IDataProvider<WorkflowInstance> _instanceRepository;
        private readonly IDataProvider<WorkflowTemplate> _templateRepository;
        private readonly IDataProvider<TaskInstance> _taskInstanceRepository;
        private readonly ILogger<IWorkflowEngine> _logger;

        public WorkflowEngine(
            IDataProvider<WorkflowInstance> instanceRepository,
            IDataProvider<WorkflowTemplate> templateRepository,
            IDataProvider<TaskInstance> taskInstanceRepository,
            ILogger<IWorkflowEngine> logger)
        {
            _instanceRepository = instanceRepository;
            _templateRepository = templateRepository;
            _taskInstanceRepository = taskInstanceRepository;
            _logger = logger;
        }

        public async Task<WorkflowInstance> StartInstance(CreateInstance request)
        {
            var template = await _templateRepository.GetById(request.WorkflowTemplate.Id);
            if (template == null)
                throw new KeyNotFoundException($"Workflow template with ID {request.WorkflowTemplate.Id} not found");

            var workflowInstance = new WorkflowInstance
            {
                WorkflowTemplateId = request.WorkflowTemplate.Id,
                Context = request.Context,
                Status = WorkflowStatus.Running,
                StartedAt = DateTime.UtcNow
            };

            var createdInstance = await _instanceRepository.Create(workflowInstance);

            // Create initial task instances for start tasks
            var startTasks = template.Tasks.Where(t => t.IsStartTask).ToList();
            foreach (var startTask in startTasks)
                await CreateTaskInstanceAsync(createdInstance.Id, startTask);

            return await GetInstance(createdInstance.Id);
        }

        public async Task<WorkflowInstance> GetInstance(int instanceId)
        {
            var instance = await _instanceRepository.GetById(instanceId);
            if (instance == null)
                throw new KeyNotFoundException($"Workflow instance with ID {instanceId} not found");

            return instance;
        }

        public async Task<IEnumerable<WorkflowInstance>> GetInstances(int workflowTemplateId, Context context = null)
        {
            var item = Expression.Parameter(typeof(WorkflowInstance));
            var propertyValue = Expression.Property(item, nameof(WorkflowInstance.WorkflowTemplateId));
            var body = Expression.Equal(propertyValue, Expression.Constant(workflowTemplateId));
            var expression = Expression.Lambda<Func<WorkflowInstance, bool>>(body, item);

            var instances = await _instanceRepository.GetAll(expression);

            if (context != null)
                instances = instances.Where(i => i.Context.Id == context.Id && i.Context.Type == context.Type).ToList();

            return instances;
        }

        public async Task CompleteTask(CompleteTask request)
        {
            var taskInstance = await _taskInstanceRepository.GetById(request.TaskInstanceId);
            if (taskInstance == null)
                throw new KeyNotFoundException($"Task instance with ID {request.TaskInstanceId} not found");

            if (taskInstance.Status != TaskInstanceStatus.Pending && taskInstance.Status != TaskInstanceStatus.Running)
                throw new InvalidOperationException($"Task instance is in {taskInstance.Status} status and cannot be completed");

            // Update task instance
            taskInstance.Status = TaskInstanceStatus.Completed;
            taskInstance.CompletedAt = DateTime.UtcNow;
            taskInstance.Outcome = request.Outcome;
            taskInstance.Output = request.Output;

            await _taskInstanceRepository.Update(taskInstance);

            // Process transitions
            await ProcessTransitionsAsync(taskInstance);

            // Check if workflow is complete
            await CheckWorkflowCompletionAsync(taskInstance.WorkflowInstanceId);
        }

        public async Task ProcessPendingTasks(int WorkflowInstanceId)
        {
            var item = Expression.Parameter(typeof(TaskInstance));
            var propertyValue = Expression.Property(item, nameof(TaskInstance.WorkflowInstanceId));
            var body = Expression.Equal(propertyValue, Expression.Constant(WorkflowInstanceId));
            var expression = Expression.Lambda<Func<TaskInstance, bool>>(body, item);

            var pendingTasks = await _taskInstanceRepository.GetAll(expression);

            foreach (var task in pendingTasks.Where(t => t.Status == TaskInstanceStatus.Pending))
                // Check if task should start (e.g., delay has passed)
                if (task.DueAt.HasValue && task.DueAt.Value <= DateTime.UtcNow)
                {
                    task.Status = TaskInstanceStatus.Running;
                    task.StartedAt = DateTime.UtcNow;
                    await _taskInstanceRepository.Update(task);

                    // For automatic tasks, process them immediately
                    if (task.WorkflowTask.Type == TaskType.Automatic)
                        await ProcessAutomaticTaskAsync(task);
                }
        }

        public async Task<TaskInstance> GetTask(int taskInstanceId)
        {
            var taskInstance = await _taskInstanceRepository.GetById(taskInstanceId);
            if (taskInstance == null)
                throw new KeyNotFoundException($"Task instance with ID {taskInstanceId} not found");

            return taskInstance;
        }

        private async Task CreateTaskInstanceAsync(int workflowInstanceId, WorkflowTask task)
        {
            var taskInstance = new TaskInstance
            {
                WorkflowInstanceId = workflowInstanceId,
                WorkflowTaskId = task.Id,
                Status = TaskInstanceStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // Set due date if task has delay
            if (task.DelayInMinutes.HasValue)
                taskInstance.DueAt = DateTime.UtcNow.AddMinutes(task.DelayInMinutes.Value);
            else
                taskInstance.DueAt = DateTime.UtcNow;

            await _taskInstanceRepository.Create(taskInstance);
        }

        private async Task ProcessTransitionsAsync(TaskInstance completedTask)
        {
            var workflowInstance = await _instanceRepository.GetById(completedTask.WorkflowInstanceId);
            var transitions = workflowInstance.WorkflowTemplate.Transitions
                .Where(t => t.FromTaskId == completedTask.WorkflowTaskId)
                .ToList();

            foreach (var transition in transitions)
            {
                var shouldTransition = false;

                switch (transition.Trigger.TriggerType)
                {
                    case TriggerType.TaskOutcome:
                        shouldTransition = completedTask.Outcome.Equals(transition.Trigger.TriggerValue);
                        break;

                    case TriggerType.Manual:
                        // Manual transitions are handled separately
                        break;

                    default:
                        shouldTransition = true; // Default behavior
                        break;
                }

                if (shouldTransition)
                {
                    var toTask = workflowInstance.WorkflowTemplate.Tasks.First(t => t.Id == transition.ToTaskId);

                    // Check if task instance already exists
                    var existingTaskInstance = workflowInstance.TaskInstances
                        .FirstOrDefault(ti => ti.WorkflowTaskId == toTask.Id &&
                                            (ti.Status == TaskInstanceStatus.Pending || ti.Status == TaskInstanceStatus.Running));

                    if (existingTaskInstance == null)
                        await CreateTaskInstanceAsync(workflowInstance.Id, toTask);
                }
            }
        }

        private async Task CheckWorkflowCompletionAsync(int workflowInstanceId)
        {
            var workflowInstance = await _instanceRepository.GetById(workflowInstanceId);
            var endTasks = workflowInstance.WorkflowTemplate.Tasks.Where(t => t.IsEndTask).ToList();

            // Check if all end tasks are completed
            var allEndTasksCompleted = endTasks.All(endTask =>
                workflowInstance.TaskInstances.Any(ti =>
                    ti.WorkflowTaskId == endTask.Id &&
                    ti.Status == TaskInstanceStatus.Completed));

            if (allEndTasksCompleted)
            {
                workflowInstance.Status = WorkflowStatus.Completed;
                workflowInstance.CompletedAt = DateTime.UtcNow;
                await _instanceRepository.Update(workflowInstance);
            }
        }

        private async Task ProcessAutomaticTaskAsync(TaskInstance taskInstance)
        {
            try
            {
                // Simulate automatic task processing
                await Task.Delay(1000); // Simulate processing time

                taskInstance.Status = TaskInstanceStatus.Completed;
                taskInstance.CompletedAt = DateTime.UtcNow;
                //taskInstance.Outcome = ;
                taskInstance.Output = JsonSerializer.Serialize(new { ProcessedAt = DateTime.UtcNow });

                await _taskInstanceRepository.Update(taskInstance);
                await ProcessTransitionsAsync(taskInstance);
                await CheckWorkflowCompletionAsync(taskInstance.WorkflowInstanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing automatic task {TaskInstanceId}", taskInstance.Id);

                taskInstance.Status = TaskInstanceStatus.Failed;
                taskInstance.ErrorMessage = ex.Message;
                await _taskInstanceRepository.Update(taskInstance);
            }
        }
    }
}
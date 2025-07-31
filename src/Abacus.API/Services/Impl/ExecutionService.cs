using Abacus.API.Contracts;
using Abacus.API.Model;
using Abacus.API.Repositories;
using AutoMapper;
using Newtonsoft.Json;

namespace Abacus.API.Services.Impl
{
    public class ExecutionService : IExecutionService
    {
        private readonly IInstanceRepository _instanceRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITaskInstanceRepository _taskInstanceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ExecutionService> _logger;

        public ExecutionService(
            IInstanceRepository instanceRepository,
            ITemplateRepository templateRepository,
            ITaskInstanceRepository taskInstanceRepository,
            IMapper mapper,
            ILogger<ExecutionService> logger)
        {
            _instanceRepository = instanceRepository;
            _templateRepository = templateRepository;
            _taskInstanceRepository = taskInstanceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Contracts.WorkflowInstance> StartWorkflowAsync(CreateWorkflowInstanceRequest request)
        {
            var template = await _templateRepository.GetByIdWithDetailsAsync(request.WorkflowTemplateId);
            if (template == null)
                throw new KeyNotFoundException($"Workflow template with ID {request.WorkflowTemplateId} not found");

            var workflowInstance = new Model.WorkflowInstance
            {
                WorkflowTemplateId = request.WorkflowTemplateId,
                EntityId = request.EntityId,
                EntityType = request.EntityType,
                Context = request.Context,
                Status = Model.WorkflowStatus.Running,
                StartedAt = DateTime.UtcNow
            };

            var createdInstance = await _instanceRepository.CreateAsync(workflowInstance);

            // Create initial task instances for start tasks
            var startTasks = template.Tasks.Where(t => t.IsStartTask).ToList();
            foreach (var startTask in startTasks)
                await CreateTaskInstanceAsync(createdInstance.Id, startTask);

            return await GetWorkflowInstanceAsync(createdInstance.Id);
        }

        public async Task<Contracts.WorkflowInstance> GetWorkflowInstanceAsync(int instanceId)
        {
            var instance = await _instanceRepository.GetByIdWithDetailsAsync(instanceId);
            if (instance == null)
                throw new KeyNotFoundException($"Workflow instance with ID {instanceId} not found");

            return _mapper.Map<Contracts.WorkflowInstance>(instance);
        }

        public async Task<IEnumerable<Contracts.WorkflowInstance>> GetAllWorkflowInstancesAsync()
        {
            var instances = await _instanceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Contracts.WorkflowInstance>>(instances);
        }

        public async Task<IEnumerable<Contracts.WorkflowInstance>> GetWorkflowInstancesByEntityAsync(string entityId, string entityType)
        {
            var instances = await _instanceRepository.GetByEntityAsync(entityId, entityType);
            return _mapper.Map<IEnumerable<Contracts.WorkflowInstance>>(instances);
        }

        public async Task CompleteTaskAsync(CompleteTaskRequest request)
        {
            var taskInstance = await _taskInstanceRepository.GetByIdAsync(request.TaskInstanceId);
            if (taskInstance == null)
                throw new KeyNotFoundException($"Task instance with ID {request.TaskInstanceId} not found");

            if (taskInstance.Status != Model.TaskInstanceStatus.Pending && taskInstance.Status != Model.TaskInstanceStatus.Running)
                throw new InvalidOperationException($"Task instance is in {taskInstance.Status} status and cannot be completed");

            // Update task instance
            taskInstance.Status = Model.TaskInstanceStatus.Completed;
            taskInstance.CompletedAt = DateTime.UtcNow;
            taskInstance.Outcome = request.Outcome;
            taskInstance.Output = request.Output;

            await _taskInstanceRepository.UpdateAsync(taskInstance);

            // Process transitions
            await ProcessTransitionsAsync(taskInstance);

            // Check if workflow is complete
            await CheckWorkflowCompletionAsync(taskInstance.WorkflowInstanceId);
        }

        public async Task ProcessPendingTasksAsync()
        {
            var pendingTasks = await _taskInstanceRepository.GetPendingTasksAsync();

            foreach (var task in pendingTasks.Where(t => t.Status == Model.TaskInstanceStatus.Pending))
                // Check if task should start (e.g., delay has passed)
                if (task.DueAt.HasValue && task.DueAt.Value <= DateTime.UtcNow)
                {
                    task.Status = Model.TaskInstanceStatus.Running;
                    task.StartedAt = DateTime.UtcNow;
                    await _taskInstanceRepository.UpdateAsync(task);

                    // For automatic tasks, process them immediately
                    if (task.WorkflowTask.Type == TaskType.Automatic)
                        await ProcessAutomaticTaskAsync(task);
                }
        }

        public async Task<Contracts.TaskInstance> GetTaskInstanceAsync(int taskInstanceId)
        {
            var taskInstance = await _taskInstanceRepository.GetByIdAsync(taskInstanceId);
            if (taskInstance == null)
                throw new KeyNotFoundException($"Task instance with ID {taskInstanceId} not found");

            return _mapper.Map<Contracts.TaskInstance>(taskInstance);
        }

        private async Task CreateTaskInstanceAsync(int workflowInstanceId, Model.WorkflowTask task)
        {
            var taskInstance = new Model.TaskInstance
            {
                WorkflowInstanceId = workflowInstanceId,
                WorkflowTaskId = task.Id,
                Status = Model.TaskInstanceStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // Set due date if task has delay
            if (task.DelayInMinutes.HasValue)
                taskInstance.DueAt = DateTime.UtcNow.AddMinutes(task.DelayInMinutes.Value);
            else
                taskInstance.DueAt = DateTime.UtcNow;

            await _taskInstanceRepository.CreateAsync(taskInstance);
        }

        private async Task ProcessTransitionsAsync(Model.TaskInstance completedTask)
        {
            var workflowInstance = await _instanceRepository.GetByIdWithDetailsAsync(completedTask.WorkflowInstanceId);
            var transitions = workflowInstance.WorkflowTemplate.Transitions
                .Where(t => t.FromTaskId == completedTask.WorkflowTaskId)
                .ToList();

            foreach (var transition in transitions)
            {
                var shouldTransition = false;

                switch (transition.TriggerType)
                {
                    case Model.TriggerType.TaskOutcome:
                        shouldTransition = string.Equals(completedTask.Outcome, transition.TriggerValue, StringComparison.OrdinalIgnoreCase);
                        break;

                    case Model.TriggerType.Manual:
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
                                            (ti.Status == Model.TaskInstanceStatus.Pending || ti.Status == Model.TaskInstanceStatus.Running));

                    if (existingTaskInstance == null)
                        await CreateTaskInstanceAsync(workflowInstance.Id, toTask);
                }
            }
        }

        private async Task CheckWorkflowCompletionAsync(int workflowInstanceId)
        {
            var workflowInstance = await _instanceRepository.GetByIdWithDetailsAsync(workflowInstanceId);
            var endTasks = workflowInstance.WorkflowTemplate.Tasks.Where(t => t.IsEndTask).ToList();

            // Check if all end tasks are completed
            var allEndTasksCompleted = endTasks.All(endTask =>
                workflowInstance.TaskInstances.Any(ti =>
                    ti.WorkflowTaskId == endTask.Id &&
                    ti.Status == Model.TaskInstanceStatus.Completed));

            if (allEndTasksCompleted)
            {
                workflowInstance.Status = Model.WorkflowStatus.Completed;
                workflowInstance.CompletedAt = DateTime.UtcNow;
                await _instanceRepository.UpdateAsync(workflowInstance);
            }
        }

        private async Task ProcessAutomaticTaskAsync(Model.TaskInstance taskInstance)
        {
            try
            {
                // Simulate automatic task processing
                await Task.Delay(1000); // Simulate processing time

                taskInstance.Status = Model.TaskInstanceStatus.Completed;
                taskInstance.CompletedAt = DateTime.UtcNow;
                taskInstance.Outcome = "Success";
                taskInstance.Output = JsonConvert.SerializeObject(new { ProcessedAt = DateTime.UtcNow });

                await _taskInstanceRepository.UpdateAsync(taskInstance);
                await ProcessTransitionsAsync(taskInstance);
                await CheckWorkflowCompletionAsync(taskInstance.WorkflowInstanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing automatic task {TaskInstanceId}", taskInstance.Id);

                taskInstance.Status = Model.TaskInstanceStatus.Failed;
                taskInstance.ErrorMessage = ex.Message;
                await _taskInstanceRepository.UpdateAsync(taskInstance);
            }
        }
    }
}
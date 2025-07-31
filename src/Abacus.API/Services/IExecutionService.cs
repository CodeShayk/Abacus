using Abacus.API.Contracts;

namespace Abacus.API.Services
{
    public interface IExecutionService
    {
        Task<WorkflowInstance> StartWorkflowAsync(CreateWorkflowInstanceRequest request);

        Task<WorkflowInstance> GetWorkflowInstanceAsync(int instanceId);

        Task<IEnumerable<WorkflowInstance>> GetAllWorkflowInstancesAsync();

        Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesByEntityAsync(string entityId, string entityType);

        Task CompleteTaskAsync(CompleteTaskRequest request);

        Task ProcessPendingTasksAsync();

        Task<TaskInstance> GetTaskInstanceAsync(int taskInstanceId);
    }
}
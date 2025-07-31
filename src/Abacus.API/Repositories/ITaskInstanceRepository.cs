using Abacus.API.Model;

namespace Abacus.API.Repositories
{
    public interface ITaskInstanceRepository
    {
        Task<TaskInstance> GetByIdAsync(int id);

        Task<TaskInstance> CreateAsync(TaskInstance taskInstance);

        Task<TaskInstance> UpdateAsync(TaskInstance taskInstance);

        Task<IEnumerable<TaskInstance>> GetPendingTasksAsync();

        Task<IEnumerable<TaskInstance>> GetTasksByWorkflowInstanceAsync(int workflowInstanceId);
    }
}
using Abacus.API.Model;

namespace Abacus.API.Repositories
{
    public interface IInstanceRepository
    {
        Task<IEnumerable<WorkflowInstance>> GetAllAsync();

        Task<WorkflowInstance> GetByIdAsync(int id);

        Task<WorkflowInstance> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<WorkflowInstance>> GetByEntityAsync(string entityId, string entityType);

        Task<WorkflowInstance> CreateAsync(WorkflowInstance instance);

        Task<WorkflowInstance> UpdateAsync(WorkflowInstance instance);

        Task DeleteAsync(int id);

        Task<IEnumerable<WorkflowInstance>> GetRunningInstancesAsync();
    }
}
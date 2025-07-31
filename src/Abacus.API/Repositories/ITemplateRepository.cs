using Abacus.API.Model;

namespace Abacus.API.Repositories
{
    public interface ITemplateRepository
    {
        Task<IEnumerable<WorkflowTemplate>> GetAllAsync();

        Task<WorkflowTemplate> GetByIdAsync(int id);

        Task<WorkflowTemplate> GetByIdWithDetailsAsync(int id);

        Task<WorkflowTemplate> CreateAsync(WorkflowTemplate template);

        Task<WorkflowTemplate> UpdateAsync(WorkflowTemplate template);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
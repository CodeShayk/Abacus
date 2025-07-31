using Abacus.API.Contracts;

namespace Abacus.API.Services
{
    public interface ITemplateService
    {
        Task<IEnumerable<WorkflowTemplate>> GetAllTemplatesAsync();

        Task<WorkflowTemplate> GetTemplateByIdAsync(int id);

        Task<WorkflowTemplate> CreateTemplateAsync(WorkflowTemplate templateDto);

        Task<WorkflowTemplate> UpdateTemplateAsync(int id, WorkflowTemplate templateDto);

        Task DeleteTemplateAsync(int id);

        Task<bool> ValidateTemplateAsync(WorkflowTemplate templateDto);
    }
}
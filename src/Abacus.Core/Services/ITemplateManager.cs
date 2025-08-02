using Abacus.Core.Model;

namespace Abacus.Core.Services
{
    /// <summary>
    /// Interface for managing workflow templates.
    /// </summary>
    public interface ITemplateManager
    {
        /// <summary>
        /// Retrieves all workflow templates.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<WorkflowTemplate>> GetTemplates();

        /// <summary>
        /// Retrieves a specific workflow template by its identifier.
        /// </summary>
        /// <param name="id">Workflow Template Identifier</param>
        /// <returns></returns>
        Task<WorkflowTemplate> GetTemplate(int id);

        /// <summary>
        /// Creates a new workflow template.
        /// </summary>
        /// <param name="template">Workflow Template</param>
        /// <returns></returns>
        Task<WorkflowTemplate> CreateTemplate(WorkflowTemplate template);

        /// <summary>
        /// Updates an existing workflow template.
        /// </summary>
        /// <param name="template">Workflow Template</param>
        /// <returns></returns>
        Task<WorkflowTemplate> UpdateTemplate(WorkflowTemplate template);

        /// <summary>
        /// Deletes a workflow template by its identifier.
        /// </summary>
        /// <param name="id">Workflow Template Identifier</param>
        /// <returns></returns>
        Task DeleteTemplate(int id);

        /// <summary>
        /// Validates the configuration of a workflow template.
        /// </summary>
        /// <param name="template">Workflow Template</param>
        /// <returns></returns>
        Task<bool> ValidateTemplate(WorkflowTemplate template);
    }
}
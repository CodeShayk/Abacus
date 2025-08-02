using Abacus.Core.Model;

namespace Abacus.Core.Services.Impl
{
    public class TemplateManager : ITemplateManager
    {
        private readonly IDataProvider<WorkflowTemplate> templateProvider;

        public TemplateManager(IDataProvider<WorkflowTemplate> templateProvider)
        {
            this.templateProvider = templateProvider;
        }

        public async Task<IEnumerable<WorkflowTemplate>> GetTemplates()
        {
            var templates = await templateProvider.GetAll();
            return templates;
        }

        public async Task<WorkflowTemplate> GetTemplate(int id)
        {
            var template = await templateProvider.GetById(id);
            if (template == null)
                throw new KeyNotFoundException($"Workflow template with ID {id} not found");

            return template;
        }

        public async Task<WorkflowTemplate> CreateTemplate(WorkflowTemplate template)
        {
            if (!await ValidateTemplate(template))
                throw new ArgumentException("Invalid workflow template configuration");

            var createdTemplate = await templateProvider.Create(template);
            return createdTemplate;
        }

        public async Task<WorkflowTemplate> UpdateTemplate(WorkflowTemplate template)
        {
            if (template == null)
                throw new ArgumentNullException($"Workflow template was null");

            if (template.Id <= 0)
                throw new ArgumentException($"Workflow template ID must be greater than zero");

            var existingTemplate = await templateProvider.GetById(template.Id);
            if (existingTemplate == null)
                throw new KeyNotFoundException($"Workflow template with ID {template.Id} not found");

            if (!await ValidateTemplate(template))
                throw new ArgumentException("Invalid workflow template configuration");

            var updatedTemplate = await templateProvider.Update(template);
            return updatedTemplate;
        }

        public async Task DeleteTemplate(int id)
        {
            var existingTemplate = await templateProvider.GetById(id);

            if (existingTemplate == null)
                throw new KeyNotFoundException($"Workflow template with ID {id} not found");

            await templateProvider.Delete(id);
        }

        public async Task<bool> ValidateTemplate(WorkflowTemplate templateDto)
        {
            // Basic validation rules
            if (string.IsNullOrWhiteSpace(templateDto.Name))
                return false;

            if (templateDto.Tasks == null || !templateDto.Tasks.Any())
                return false;

            // Must have at least one start task
            if (!templateDto.Tasks.Any(t => t.IsStartTask))
                return false;

            // Must have at least one end task
            if (!templateDto.Tasks.Any(t => t.IsEndTask))
                return false;

            // Validate transitions
            if (templateDto.Transitions != null)
                foreach (var transition in templateDto.Transitions)
                {
                    var fromTaskExists = templateDto.Tasks.Any(t => t.Id == transition.FromTask.Id);
                    var toTaskExists = templateDto.Tasks.Any(t => t.Id == transition.ToTask.Id);

                    if (!fromTaskExists || !toTaskExists)
                        return false;
                }

            return true;
        }
    }
}
using Abacus.API.Repositories;
using AutoMapper;

namespace Abacus.API.Services.Impl
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IMapper _mapper;

        public TemplateService(ITemplateRepository templateRepository, IMapper mapper)
        {
            _templateRepository = templateRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Contracts.WorkflowTemplate>> GetAllTemplatesAsync()
        {
            var templates = await _templateRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Contracts.WorkflowTemplate>>(templates);
        }

        public async Task<Contracts.WorkflowTemplate> GetTemplateByIdAsync(int id)
        {
            var template = await _templateRepository.GetByIdWithDetailsAsync(id);
            if (template == null)
                throw new KeyNotFoundException($"Workflow template with ID {id} not found");

            return _mapper.Map<Contracts.WorkflowTemplate>(template);
        }

        public async Task<Contracts.WorkflowTemplate> CreateTemplateAsync(Contracts.WorkflowTemplate templateDto)
        {
            if (!await ValidateTemplateAsync(templateDto))
                throw new ArgumentException("Invalid workflow template configuration");

            var template = _mapper.Map<Model.WorkflowTemplate>(templateDto);
            var createdTemplate = await _templateRepository.CreateAsync(template);
            return _mapper.Map<Contracts.WorkflowTemplate>(createdTemplate);
        }

        public async Task<Contracts.WorkflowTemplate> UpdateTemplateAsync(int id, Contracts.WorkflowTemplate templateDto)
        {
            var existingTemplate = await _templateRepository.GetByIdAsync(id);
            if (existingTemplate == null)
                throw new KeyNotFoundException($"Workflow template with ID {id} not found");

            if (!await ValidateTemplateAsync(templateDto))
                throw new ArgumentException("Invalid workflow template configuration");

            _mapper.Map(templateDto, existingTemplate);
            var updatedTemplate = await _templateRepository.UpdateAsync(existingTemplate);
            return _mapper.Map<Contracts.WorkflowTemplate>(updatedTemplate);
        }

        public async Task DeleteTemplateAsync(int id)
        {
            if (!await _templateRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Workflow template with ID {id} not found");

            await _templateRepository.DeleteAsync(id);
        }

        public async Task<bool> ValidateTemplateAsync(Contracts.WorkflowTemplate templateDto)
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
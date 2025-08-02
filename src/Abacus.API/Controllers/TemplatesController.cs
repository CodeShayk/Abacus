using Abacus.API.Contracts;
using Abacus.Core.Model;
using Abacus.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abacus.API.Controllers
{
    [ApiController]
    [Route("api/workflow/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateManager _templateManager;
        private readonly ILogger<TemplatesController> _logger;

        public TemplatesController(ITemplateManager templateManager, ILogger<TemplatesController> logger)
        {
            _templateManager = templateManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contracts.WorkflowTemplate>>> GetAllTemplates()
        {
            try
            {
                var templates = await _templateManager.GetTemplates();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow templates");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contracts.WorkflowTemplate>> GetTemplate(int id)
        {
            try
            {
                var template = await _templateManager.GetTemplate(id);
                return Ok(template);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Workflow template with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow template {TemplateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Contracts.WorkflowTemplate>> CreateTemplate([FromBody] Contracts.WorkflowTemplate template)
        {
            try
            {
                var createdTemplate = await _templateManager.CreateTemplate(new Core.Model.WorkflowTemplate
                {
                    Name = template.Name,
                    Description = template.Description,
                    Tasks = template.Tasks?.Select(x => new Core.Model.WorkflowTask
                    {
                        Name = x.Name,
                        Description = x.Description,
                        IsStartTask = x.IsStartTask,
                        IsEndTask = x.IsEndTask,
                        Type = Enum.Parse<TaskType>(x.Type)
                    }).ToList() ?? new List<Core.Model.WorkflowTask>(),
                    Transitions = template.Transitions?.Select(x => new Core.Model.WorkflowTransition
                    {
                        FromTaskId = x.FromTask.Id,
                        ToTaskId = x.ToTask.Id,
                        Condition = new Core.Model.TriggerCondition { Condition = x.Condition?.Condition },
                    }).ToList() ?? new List<Core.Model.WorkflowTransition>()
                });
                return CreatedAtAction(nameof(GetTemplate), new { id = createdTemplate.Id }, createdTemplate);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow template");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Contracts.WorkflowTemplate>> UpdateTemplate(int id, [FromBody] Contracts.WorkflowTemplate template)
        {
            try
            {
                var updatedTemplate = await _templateManager.UpdateTemplate(new Core.Model.WorkflowTemplate
                {
                    Id = id,
                    Name = template.Name,
                    Description = template.Description,
                    Tasks = template.Tasks?.Select(x => new Core.Model.WorkflowTask
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        IsStartTask = x.IsStartTask,
                        IsEndTask = x.IsEndTask,
                        Type = Enum.Parse<TaskType>(x.Type)
                    }).ToList() ?? new List<Core.Model.WorkflowTask>(),
                    Transitions = template.Transitions?.Select(x => new Core.Model.WorkflowTransition
                    {
                        Id = x.Id,
                        FromTaskId = x.FromTask.Id,
                        ToTaskId = x.ToTask.Id,
                        Condition = new Core.Model.TriggerCondition { Condition = x.Condition?.Condition },
                    }).ToList() ?? new List<Core.Model.WorkflowTransition>()
                });

                return Ok(updatedTemplate);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Workflow template with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating workflow template {TemplateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            try
            {
                await _templateManager.DeleteTemplate(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Workflow template with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow template {TemplateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/validate")]
        public async Task<ActionResult<bool>> ValidateTemplate(int id)
        {
            try
            {
                var template = await _templateManager.GetTemplate(id);
                var isValid = await _templateManager.ValidateTemplate(template);
                return Ok(new { IsValid = isValid });
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Workflow template with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating workflow template {TemplateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
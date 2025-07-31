using Abacus.API.Contracts;
using Abacus.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abacus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly ILogger<TemplatesController> _logger;

        public TemplatesController(ITemplateService templateService, ILogger<TemplatesController> logger)
        {
            _templateService = templateService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkflowTemplate>>> GetAllTemplates()
        {
            try
            {
                var templates = await _templateService.GetAllTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow templates");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkflowTemplate>> GetTemplate(int id)
        {
            try
            {
                var template = await _templateService.GetTemplateByIdAsync(id);
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
        public async Task<ActionResult<WorkflowTemplate>> CreateTemplate([FromBody] WorkflowTemplate template)
        {
            try
            {
                var createdTemplate = await _templateService.CreateTemplateAsync(template);
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
        public async Task<ActionResult<WorkflowTemplate>> UpdateTemplate(int id, [FromBody] WorkflowTemplate template)
        {
            try
            {
                var updatedTemplate = await _templateService.UpdateTemplateAsync(id, template);
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
                await _templateService.DeleteTemplateAsync(id);
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
                var template = await _templateService.GetTemplateByIdAsync(id);
                var isValid = await _templateService.ValidateTemplateAsync(template);
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
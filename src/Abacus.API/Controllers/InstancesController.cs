using Abacus.API.Contracts;
using Abacus.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abacus.API.Controllers
{
    [ApiController]
    [Route("api/workflow")]
    public class InstancesController : ControllerBase
    {
        private readonly IWorkflowEngine _workflowEngine;
        private readonly ILogger<InstancesController> _logger;

        public InstancesController(IWorkflowEngine workflowEngine, ILogger<InstancesController> logger)
        {
            _workflowEngine = workflowEngine;
            _logger = logger;
        }

        [HttpPost("instances")]
        public async Task<ActionResult<WorkflowInstance>> StartWorkflow([FromBody] CreateInstanceRequest request)
        {
            try
            {
                var instance = await _workflowEngine.StartInstance(new Core.Model.CreateInstance
                {
                    WorkflowTemplate = new Core.Model.TemplateReference
                    {
                        Id = request.WorkflowTemplate.Id,
                        Name = request.WorkflowTemplate.Name
                    },
                    Context = new Core.Model.Context
                    {
                        Id = request.Context.Id,
                        Type = request.Context.Type,
                    }
                });
                return CreatedAtAction(nameof(GetInstance), new { id = instance.Id }, instance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting workflow instance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("instances/{id}")]
        public async Task<ActionResult<WorkflowInstance>> GetInstance(int id)
        {
            try
            {
                var instance = await _workflowEngine.GetInstance(id);
                return Ok(instance);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Workflow instance with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow instance {InstanceId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("templates/{templateId}/instances")]
        public async Task<ActionResult<IEnumerable<WorkflowInstance>>> GetAllInstances(int templateId)
        {
            try
            {
                var instances = await _workflowEngine.GetInstances(templateId);
                return Ok(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow instances");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("templates/{templateId}/instances/context/{type}/{id}")]
        public async Task<ActionResult<IEnumerable<WorkflowInstance>>> GetInstancesByEntity(int templateId, [FromQuery] int id, [FromQuery] string type)
        {
            try
            {
                var instances = await _workflowEngine.GetInstances(templateId, new Core.Model.Context
                {
                    Id = id,
                    Type = type
                });

                return Ok(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving workflow instances for entity {type}/{id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
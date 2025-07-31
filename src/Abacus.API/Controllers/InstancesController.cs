using Abacus.API.Contracts;
using Abacus.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abacus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstancesController : ControllerBase
    {
        private readonly IExecutionService _executionService;
        private readonly ILogger<InstancesController> _logger;

        public InstancesController(IExecutionService executionService, ILogger<InstancesController> logger)
        {
            _executionService = executionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkflowInstance>>> GetAllInstances()
        {
            try
            {
                var instances = await _executionService.GetAllWorkflowInstancesAsync();
                return Ok(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow instances");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkflowInstance>> GetInstance(int id)
        {
            try
            {
                var instance = await _executionService.GetWorkflowInstanceAsync(id);
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

        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<ActionResult<IEnumerable<WorkflowInstance>>> GetInstancesByEntity(string entityType, string entityId)
        {
            try
            {
                var instances = await _executionService.GetWorkflowInstancesByEntityAsync(entityId, entityType);
                return Ok(instances);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow instances for entity {EntityType}/{EntityId}", entityType, entityId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<WorkflowInstance>> StartWorkflow([FromBody] CreateWorkflowInstanceRequest request)
        {
            try
            {
                var instance = await _executionService.StartWorkflowAsync(request);
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

        [HttpPost("tasks/complete")]
        public async Task<IActionResult> CompleteTask([FromBody] CompleteTaskRequest request)
        {
            try
            {
                await _executionService.CompleteTaskAsync(request);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task {TaskInstanceId}", request.TaskInstanceId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tasks/{taskInstanceId}")]
        public async Task<ActionResult<TaskInstance>> GetTaskInstance(int taskInstanceId)
        {
            try
            {
                var taskInstance = await _executionService.GetTaskInstanceAsync(taskInstanceId);
                return Ok(taskInstance);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Task instance with ID {taskInstanceId} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task instance {TaskInstanceId}", taskInstanceId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("process-pending")]
        public async Task<IActionResult> ProcessPendingTasks()
        {
            try
            {
                await _executionService.ProcessPendingTasksAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending tasks");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
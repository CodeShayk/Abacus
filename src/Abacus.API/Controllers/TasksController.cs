using Abacus.API.Contracts;
using Abacus.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abacus.API.Controllers
{
    [ApiController]
    [Route("api/workflow/instances")]
    public class TasksController : ControllerBase
    {
        private readonly IWorkflowEngine _executionService;
        private readonly ILogger<InstancesController> _logger;

        [HttpGet("tasks/{id}")]
        public async Task<ActionResult<TaskInstance>> GetTaskInstance(int id)
        {
            try
            {
                var taskInstance = await _executionService.GetTask(id);
                return Ok(taskInstance);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Task instance with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task instance {TaskInstanceId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("tasks/{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(int taskId, [FromBody] CompleteTaskRequest request)
        {
            try
            {
                await _executionService.CompleteTask(new Core.Model.CompleteTask
                {
                    TaskInstanceId = taskId,
                    Outcome = request.Outcome,
                    Output = request.Output
                });
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
                _logger.LogError(ex, $"Error completing task {taskId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{instanceId}/tasks/process-pending")]
        public async Task<IActionResult> ProcessPendingTasks(int instanceId)
        {
            try
            {
                await _executionService.ProcessPendingTasks(instanceId);
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
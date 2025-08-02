using Abacus.API.Contracts;
using Abacus.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abacus.API.Controllers
{
    [ApiController]
    [Route("api/workflow/instances/[controller]")]
    public class TriggersController : ControllerBase
    {
        private readonly IWorkflowEngine _executionService;
        private readonly ILogger<InstancesController> _logger;

        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] TriggerRequest request)
        {
            try
            {
                //await _workflowEngine.PublishTrigger(request);
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publish trigger {request.Trigger.Value} for entity {request.Context.Type}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
using Abacus.Core.Services;
using DomainEvents;

namespace Abacus.Core.Messaging
{
    internal class TaskCompletedHandler : IHandler<TaskCompleted>
    {
        private IWorkflowEngine workflowEngine;

        public TaskCompletedHandler(IWorkflowEngine workflowEngine) => this.workflowEngine = workflowEngine;

        public async Task HandleAsync(TaskCompleted @event)
        {
            await workflowEngine.ProgressWorkflows(@event.Payload);
        }
    }
}
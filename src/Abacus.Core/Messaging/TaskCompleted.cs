using Abacus.Core.Model;
using DomainEvents;

namespace Abacus.Core.Messaging
{
    internal class TaskCompleted: IDomainEvent
    {
        public TaskInstance Payload { get; set; }
    }
}
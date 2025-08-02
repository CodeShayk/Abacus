// Models/WorkflowTemplate.cs
// Models/WorkflowTransition.cs
namespace Abacus.Core.Model
{
    public enum TriggerType
    {
        TaskOutcome,
        DomainEvent,
        Timer,
        Manual
    }
}

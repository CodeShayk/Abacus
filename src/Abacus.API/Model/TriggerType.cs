// Models/WorkflowTemplate.cs
// Models/WorkflowTransition.cs
namespace Abacus.API.Model
{
    public enum TriggerType
    {
        TaskOutcome,
        DomainEvent,
        Timer,
        Manual
    }
}

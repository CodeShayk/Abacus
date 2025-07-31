namespace Abacus.API.Contracts
{
    public class WorkflowTransition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TriggerCondition Condition { get; set; }
        public TransitionDelay Delay { get; set; }
        public Trigger Trigger { get; set; }
        public WorkflowTask FromTask { get; set; }
        public WorkflowTask ToTask { get; set; }
    }

    public class Trigger
    {
        public TriggerType Type { get; set; }
        public string Value { get; set; }
    }

    public enum TriggerType
    {
        TaskOutcome,
        DomainEvent,
        Timer,
        Manual
    }

    public class TriggerCondition
    {
        public string Condition { get; set; }
    }

    public class TransitionDelay
    {
        public int? InMinutes { get; set; }
    }
}
namespace Abacus.Core.Model
{
    public abstract class TaskOutcome : ITriggerValue
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
// Models/WorkflowTemplate.cs
// Models/Trigger.cs
namespace Abacus.Core.Model
{
    public class Trigger
    {
        public TriggerType TriggerType { get; set; }

        public ITriggerValue TriggerValue { get; set; } // Outcome value or event name
    }
}
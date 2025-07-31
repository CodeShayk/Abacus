// Models/WorkflowTemplate.cs
// Models/WorkflowTransition.cs
namespace Abacus.API.Model
{
    public class WorkflowTransition
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Condition { get; set; } // JSON condition or expression

        public TriggerType TriggerType { get; set; }

        public string TriggerValue { get; set; } // Outcome value or event name

        // Foreign Keys
        public int WorkflowTemplateId { get; set; }

        public int FromTaskId { get; set; }
        public int ToTaskId { get; set; }

        // Navigation properties
        public virtual WorkflowTemplate WorkflowTemplate { get; set; }

        public virtual WorkflowTask FromTask { get; set; }
        public virtual WorkflowTask ToTask { get; set; }
    }
}

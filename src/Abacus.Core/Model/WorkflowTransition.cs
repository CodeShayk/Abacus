// Models/WorkflowTemplate.cs
// Models/WorkflowTransition.cs
namespace Abacus.Core.Model
{
    public class WorkflowTransition
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TriggerCondition Condition { get; set; } // JSON condition or expression

        public Trigger Trigger { get; set; }

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
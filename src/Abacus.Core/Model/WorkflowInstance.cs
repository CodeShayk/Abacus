namespace Abacus.Core.Model
{
    public class WorkflowInstance
    {
        public int Id { get; set; }

        public Context Context { get; set; } // ID of the entity this workflow is running for
        public WorkflowStatus Status { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public string ErrorMessage { get; set; }

        // Foreign Keys
        public int WorkflowTemplateId { get; set; }

        // Navigation properties
        public virtual WorkflowTemplate WorkflowTemplate { get; set; }

        public virtual ICollection<TaskInstance> TaskInstances { get; set; } = new List<TaskInstance>();
    }
}
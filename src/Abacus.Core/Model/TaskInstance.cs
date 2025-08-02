namespace Abacus.Core.Model
{
    public class TaskInstance
    {
        public int Id { get; set; }

        public TaskInstanceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? DueAt { get; set; }

        public string Input { get; set; } // JSON input data

        public string Output { get; set; } // JSON output data

        public TaskOutcome Outcome { get; set; }

        public string ErrorMessage { get; set; }

        public string AssignedTo { get; set; } // User or system component

        // Foreign Keys
        public int WorkflowInstanceId { get; set; }

        public int WorkflowTaskId { get; set; }

        // Navigation properties
        public virtual WorkflowInstance WorkflowInstance { get; set; }

        public virtual WorkflowTask WorkflowTask { get; set; }
    }
}
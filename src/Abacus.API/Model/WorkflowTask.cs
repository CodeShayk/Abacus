// Models/WorkflowTemplate.cs
using System.ComponentModel.DataAnnotations;

namespace Abacus.API.Model
{
    public class WorkflowTask
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public TaskType Type { get; set; }

        public string Configuration { get; set; } // JSON configuration

        public int? DelayInMinutes { get; set; }

        public bool IsStartTask { get; set; }

        public bool IsEndTask { get; set; }

        // UI positioning
        public double PositionX { get; set; }

        public double PositionY { get; set; }

        // Foreign Keys
        public int WorkflowTemplateId { get; set; }

        // Navigation properties
        public virtual WorkflowTemplate WorkflowTemplate { get; set; }

        public virtual ICollection<WorkflowTransition> FromTransitions { get; set; } = new List<WorkflowTransition>();
        public virtual ICollection<WorkflowTransition> ToTransitions { get; set; } = new List<WorkflowTransition>();
        public virtual ICollection<TaskInstance> TaskInstances { get; set; } = new List<TaskInstance>();
    }
}

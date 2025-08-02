// Models/WorkflowTemplate.cs
using System.ComponentModel.DataAnnotations;

namespace Abacus.Core.Model
{
    public class WorkflowTemplate
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<WorkflowTask> Tasks { get; set; } = new List<WorkflowTask>();

        public virtual ICollection<WorkflowTransition> Transitions { get; set; } = new List<WorkflowTransition>();
        public virtual ICollection<WorkflowInstance> Instances { get; set; } = new List<WorkflowInstance>();
    }
}
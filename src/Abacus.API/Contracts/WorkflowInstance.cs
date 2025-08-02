using Abacus.Core.Model;

namespace Abacus.API.Contracts
{
    public class WorkflowInstance
    {
        public int Id { get; set; }
        public Context Context { get; set; }
        public WorkflowStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string ErrorMessage { get; set; }
        public TemplateReference WorkflowTemplate { get; set; }
        public List<TaskInstance> TaskInstances { get; set; } = new();
    }

    public class Context
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public enum WorkflowStatus
    {
        Running,
        Completed,
        Failed,
        Suspended,
        Cancelled
    }

    public class TemplateReference
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
namespace Abacus.API.Contracts
{
    public class WorkflowInstance
    {
        public int Id { get; set; }
        public Entity Entity { get; set; }
        public WorkflowStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Context { get; set; }
        public string ErrorMessage { get; set; }
        public TemplateReference WorkflowTemplate { get; set; }
        public List<TaskInstance> TaskInstances { get; set; } = new();
    }

    public enum WorkflowStatus
    {
        Running,
        Completed,
        Failed,
        Suspended,
        Cancelled
    }

    public class Entity
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }

    public class TemplateReference
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
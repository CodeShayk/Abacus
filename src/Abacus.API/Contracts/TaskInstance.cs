using Abacus.Core.Model;

namespace Abacus.API.Contracts
{
    public class TaskInstance
    {
        public int Id { get; set; }
        public TaskInstanceStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public TaskOutcome Outcome { get; set; }
        public string ErrorMessage { get; set; }
        public string AssignedTo { get; set; }
        public int WorkflowInstanceId { get; set; }
        public TaskReference WorkflowTask { get; set; }
    }

    public enum TaskInstanceStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Suspended,
        Cancelled,
        Waiting
    }

    public class TaskReference
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class TaskOutcome : Core.Model.TaskOutcome
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
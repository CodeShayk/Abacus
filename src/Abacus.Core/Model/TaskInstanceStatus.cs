// Models/WorkflowTemplate.cs
namespace Abacus.Core.Model
{
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
}
// Models/WorkflowTemplate.cs
namespace Abacus.API.Model
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
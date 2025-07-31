// Models/WorkflowTemplate.cs
// Models/WorkflowTransition.cs

// Models/WorkflowInstance.cs
namespace Abacus.API.Model
{
    public enum WorkflowStatus
    {
        Running,
        Completed,
        Failed,
        Suspended,
        Cancelled
    }
}

// Models/WorkflowTemplate.cs
namespace Abacus.API.Model
{
    public enum TaskType
    {
        Manual,
        Automatic,
        Approval,
        Notification,
        DataProcessing,
        Delay,
        Condition
    }
}

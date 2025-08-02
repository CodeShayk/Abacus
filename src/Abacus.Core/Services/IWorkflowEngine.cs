using Abacus.Core.Model;

namespace Abacus.Core.Services
{
    /// <summary>
    /// Interface for the workflow engine that manages workflow instances and tasks.
    /// </summary>
    public interface IWorkflowEngine
    {
        /// <summary>
        /// Starts a new workflow instance based on the workflow template.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<WorkflowInstance> StartInstance(CreateInstance request);

        /// <summary>
        /// Retrieves a workflow instance by its identifier.
        /// </summary>
        /// <param name="instanceId">Workflow Instance Identifier.</param>
        /// <returns></returns>
        Task<WorkflowInstance> GetInstance(int instanceId);

        /// <summary>
        /// Retrieves all workflow instances for a specific workflow template.
        /// </summary>
        /// <param name="WorkflowId">Worklfow Template Id linked to the instances </param>
        /// <param name="context">Optional Context linked to the instances</param>
        /// <returns></returns>
        Task<IEnumerable<WorkflowInstance>> GetInstances(int WorkflowId, Context context = null);

        /// <summary>
        /// Updates an existing workflow instance.
        /// </summary>
        /// <param name="request">Completion context.</param>
        /// <returns></returns>
        Task CompleteTask(CompleteTask request);

        /// <summary>
        /// Processes pending tasks for a given workflow instance.
        /// </summary>
        /// <param name="WorkflowInstanceId">Workflow Instance Identifier</param>
        /// <returns></returns>
        Task ProcessPendingTasks(int WorkflowInstanceId);

        /// <summary>
        /// Retrieves a specific task instance by its identifier.
        /// </summary>
        /// <param name="taskId">Workflow Instance Task Identifier</param>
        /// <returns></returns>
        Task<TaskInstance> GetTask(int taskId);
    }
}
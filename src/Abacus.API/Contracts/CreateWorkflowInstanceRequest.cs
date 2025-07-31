// Data/WorkflowDbContext.cs
namespace Abacus.API.Contracts
{
    public class CreateWorkflowInstanceRequest
    {
        public int WorkflowTemplateId { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public string Context { get; set; }
    }
}
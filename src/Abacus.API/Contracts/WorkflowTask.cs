// Data/WorkflowDbContext.cs
namespace Abacus.API.Contracts
{
    public class WorkflowTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Configuration { get; set; }
        public bool IsStartTask { get; set; }
        public bool IsEndTask { get; set; }
        public TaskPosition TaskPosition { get; set; } = new TaskPosition();
    }
}
namespace Abacus.API.Contracts
{
    public class WorkflowTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public WorkflowTask[] Tasks
        {
            get { return GetTasks(); }
        }

        public WorkflowTransition[] Transitions { get; set; } = Array.Empty<WorkflowTransition>();

        private WorkflowTask[] GetTasks()
        {
            var tasks = new List<WorkflowTask>();
            foreach (var task in Transitions.Where(t => t.FromTask != null).Select(x => x.FromTask))
                if (!tasks.Any(t => t.Id == task.Id))
                    tasks.Add(task);
            foreach (var task in Transitions.Where(t => t.ToTask != null).Select(x => x.ToTask))
                if (!tasks.Any(t => t.Id == task.Id))
                    tasks.Add(task);
            return tasks.ToArray();
        }
    }
}
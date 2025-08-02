namespace Abacus.Core.Model
{
    public class CompleteTask
    {
        public int TaskInstanceId { get; set; }
        public TaskOutcome Outcome { get; set; }
        public string Output { get; set; }
    }
}
namespace Abacus.API.Contracts
{
    public class CompleteTaskRequest
    {
        public int TaskInstanceId { get; set; }
        public string Outcome { get; set; }
        public string Output { get; set; }
    }
}
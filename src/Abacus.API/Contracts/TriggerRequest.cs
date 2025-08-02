using Abacus.Core.Model;

namespace Abacus.API.Contracts
{
    public class TriggerRequest
    {
        public Context Context { get; set; }
        public Trigger Trigger { get; set; }
    }
}
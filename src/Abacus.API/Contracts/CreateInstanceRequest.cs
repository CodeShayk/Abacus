using Abacus.Core.Model;

namespace Abacus.API.Contracts
{
    public class CreateInstanceRequest
    {
        public TemplateReference WorkflowTemplate { get; set; }
        public Context Context { get; set; }
    }
}
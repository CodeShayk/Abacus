using Abacus.Core.Messaging;
using Abacus.Core.Model;
using Abacus.Core.Services;
using Abacus.Core.Services.Impl;
using DomainEvents;
using DomainEvents.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Abacus.Core
{
    public static class IocExtensions
    {
        public static void AddAbacus(this IServiceCollection services, Action<Configuration> configuration)
        {
            // Add services
            services.AddScoped<ITemplateManager, TemplateManager>();
            services.AddScoped<IWorkflowEngine, WorkflowEngine>();
            services.AddTransient<IPublisher, Publisher>();

            // register all implemented event handlers.
            services.AddTransient<IHandler, TaskCompletedHandler>();

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration), "Abacus configuration action cannot be null");

            var config = new Configuration(services);
            configuration.Invoke(config);
        }

        public class Configuration
        {
            public IServiceCollection Services { get; }

            public Configuration(IServiceCollection services)
            {
                Services = services;
            }

            public Configuration WithTemplates(Func<IServiceProvider, IDataProvider<WorkflowTemplate>> template)
            {
                if (template == null)
                    throw new ArgumentNullException(nameof(template), "Workflow Template provider cannot be null");
                Services.AddScoped(typeof(IDataProvider<WorkflowTemplate>), c => template(c));
                return this;
            }

            public Configuration WithInstances(Func<IServiceProvider, IDataProvider<WorkflowInstance>> instances)
            {
                if (instances == null)
                    throw new ArgumentNullException(nameof(instances), "Workflow Instance provider cannot be null");
                Services.AddScoped(typeof(IDataProvider<WorkflowInstance>), c => instances(c));
                return this;
            }

            public Configuration WithTasks(Func<IServiceProvider, IDataProvider<TaskInstance>> tasks)
            {
                if (tasks == null)
                    throw new ArgumentNullException(nameof(tasks), "Workflow Instance tasks provider cannot be null");
                Services.AddScoped(typeof(IDataProvider<TaskInstance>), c => tasks(c));
                return this;
            }
        }
    }
}
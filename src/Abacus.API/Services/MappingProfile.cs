using Abacus.API.Model;
using AutoMapper;

namespace Abacus.API.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // WorkflowTemplate mappings
            CreateMap<Model.WorkflowTemplate, Contracts.WorkflowTemplate>()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Instances, opt => opt.Ignore());

            // WorkflowTask mappings
            CreateMap<Model.WorkflowTask, Contracts.WorkflowTask>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<TaskType>(src.Type)))
                .ForMember(dest => dest.WorkflowTemplate, opt => opt.Ignore())
                .ForMember(dest => dest.FromTransitions, opt => opt.Ignore())
                .ForMember(dest => dest.ToTransitions, opt => opt.Ignore())
                .ForMember(dest => dest.TaskInstances, opt => opt.Ignore());

            // WorkflowTransition mappings
            CreateMap<Model.WorkflowTransition, Contracts.WorkflowTransition>()
                .ForMember(dest => dest.Trigger.Type, opt => opt.MapFrom(src => src.TriggerType.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.TriggerType, opt => opt.MapFrom(src => Enum.Parse<TriggerType>(src.Trigger.Type.ToString())))
                .ForMember(dest => dest.WorkflowTemplate, opt => opt.Ignore())
                .ForMember(dest => dest.FromTask, opt => opt.Ignore())
                .ForMember(dest => dest.ToTask, opt => opt.Ignore());

            // WorkflowInstance mappings
            CreateMap<Model.WorkflowInstance, Contracts.WorkflowInstance>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.WorkflowTemplate.Name, opt => opt.MapFrom(src => src.WorkflowTemplate.Name))
                .ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<WorkflowStatus>(src.Status.ToString())))
                .ForMember(dest => dest.WorkflowTemplate, opt => opt.Ignore());

            // TaskInstance mappings
            CreateMap<Model.TaskInstance, Contracts.TaskInstance>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.WorkflowTask.Name, opt => opt.MapFrom(src => src.WorkflowTask.Name))
                .ForMember(dest => dest.WorkflowTask.Type, opt => opt.MapFrom(src => src.WorkflowTask.Type.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<TaskInstanceStatus>(src.Status.ToString())))
                .ForMember(dest => dest.WorkflowInstance, opt => opt.Ignore())
                .ForMember(dest => dest.WorkflowTask, opt => opt.Ignore());
        }
    }
}
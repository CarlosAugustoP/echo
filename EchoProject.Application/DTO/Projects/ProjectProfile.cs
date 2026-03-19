using AutoMapper;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Application.DTO.Projects
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<SmartContractAddress, string>().ConvertUsing(src => src.Value);
            CreateMap<Project, ProjectDTO>()
                .ForMember(dest => dest.SmartContractAddress, opt => opt.MapFrom(src => src.SmartContractAddress.Value))
                .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.Goals))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(x => x.ManagerId))
                .ReverseMap();

            CreateMap<Goal, GoalDTO>(); 
            CreateMap<GoalType, GoalTypeDTO>();
        }
    }
}
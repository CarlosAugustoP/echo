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
                .ForMember(dest => dest.MainImage, opt => opt.MapFrom(src => src.MainImage != null ? src.MainImage.Url : null))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.Url).ToList()))
                .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.GetProgress()))
                .ReverseMap();
            CreateMap<Project, ProjectHeaderDTO>()
                .ForMember(dest => dest.MainImage, opt => 
                    opt.MapFrom(src => src.MainImage != null ? src.MainImage.Url : string.Empty))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.GetProgress()));

            CreateMap<Goal, GoalDTO>(); 
            CreateMap<GoalType, GoalTypeDTO>();
        }
    }
}
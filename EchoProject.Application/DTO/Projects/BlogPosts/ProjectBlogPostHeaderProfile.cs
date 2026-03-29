using AutoMapper;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Application.DTO.Projects
{
    public class ProjectBlogPostHeaderProfile : Profile
    {
        public ProjectBlogPostHeaderProfile()
        {
            CreateMap<ProjectBlogPost, ProjectBlogPostHeaderDTO>()
                .ForMember(dest => dest.HeaderImage, opt => 
                    opt.MapFrom(src => src.HeaderImage != null ? src.HeaderImage.Url : null))
                
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                
                .ReverseMap()
                
                .ForPath(dest => dest.HeaderImage, opt => 
                    opt.MapFrom(src => !string.IsNullOrEmpty(src.HeaderImage) ? new ImageUrl(src.HeaderImage) : null));
        }
    }
}
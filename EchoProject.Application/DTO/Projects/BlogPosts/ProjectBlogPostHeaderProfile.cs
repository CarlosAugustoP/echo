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
                .ForMember(dest => dest.First100CharsOfContent, opt => opt.MapFrom(src => src.Content.Length > 100 ? src.Content.Substring(0, 100) : src.Content))
                
                .ReverseMap()
                
                .ForPath(dest => dest.HeaderImage, opt => 
                    opt.MapFrom(src => !string.IsNullOrEmpty(src.HeaderImage) ? new ImageUrl(src.HeaderImage) : null));
        }
    }
}
using AutoMapper;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Application.DTO.Projects;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Application.DTO.Projects
{
    public class ProjectBlogPostProfile : Profile
    {
        public ProjectBlogPostProfile()
        {
            CreateMap<ProjectBlogPost, ProjectBlogPostDTO>()
                .ForMember(dest => dest.HeaderImage, opt => 
                    opt.MapFrom(src => src.HeaderImage != null ? src.HeaderImage.Url : null))
                
                .ForMember(dest => dest.ImagesUrls, opt => 
                    opt.MapFrom(src => src.Images.Select(img => img.Url).ToList()))

                .ForMember(dest => dest.CreatedAt , opt => opt.MapFrom(src => src.CreatedAt.ToLocalTime()))
                
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId))

                .ReverseMap()
                .ForPath(dest => dest.HeaderImage, opt => 
                    opt.MapFrom(src => !string.IsNullOrEmpty(src.HeaderImage) ? new ImageUrl(src.HeaderImage) : null))
                
                .ForPath(dest => dest.Images, opt => 
                    opt.MapFrom(src => src.ImagesUrls.Select(url => new ImageUrl(url)).ToList()))
                
                .ForMember(dest => dest.Project, opt => opt.Ignore());

            CreateMap<string, ImageUrl>().ConvertUsing(src => new ImageUrl(src));
            CreateMap<ImageUrl, string>().ConvertUsing(src => src.Url);
        }
    }
}
using AutoMapper;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Application.DTO.Projects
{
    [AutoMap(typeof(ProjectBlogPost))]
    public class ProjectBlogPostDTO
    {
        public string HeaderImage { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid ProjectId { get; private set; }
        public List<string> ImagesInBase64 { get; set; } = [];
    }
}
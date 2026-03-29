using AutoMapper;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Application.DTO.Projects
{
    public class ProjectBlogPostDTO
    {
        public Guid Id { get; private set; }
        public string? HeaderImage { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid ProjectId { get; private set; }
        public List<string> ImagesUrls { get; set; } = [];
        public DateTime CreatedAt { get; private set; }
    }
}
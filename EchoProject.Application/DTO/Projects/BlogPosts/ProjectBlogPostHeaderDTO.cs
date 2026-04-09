using AutoMapper;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Application.DTO.Projects
{
    public class ProjectBlogPostHeaderDTO
    {
        public Guid Id { get; private set; }
        public string? HeaderImage { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public string First100CharsOfContent { get; set; } = string.Empty;
    }
}
using AutoMapper;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Application.DTO.Projects
{
    public class ProjectHeaderDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string MainImage { get; set; } = string.Empty;
    }
}
using AutoMapper;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Application.DTO.Projects
{
    [AutoMap(typeof(Project))]
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ManagerId { get; set; }        
        public List<GoalDTO> Goals { get; set; } = [];
    }
}
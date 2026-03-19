using AutoMapper;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Application.DTO.Projects
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ManagerId { get; set; }        
        public List<GoalDTO> Goals { get; set; } = [];
        public string SmartContractAddress { get; set; } = string.Empty;
    }
}
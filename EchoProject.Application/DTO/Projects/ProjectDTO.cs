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
        public string? MainImage { get; set; }
        public List<string> Images { get; set; } = [];
        public decimal Progress { get; set; } = 0;
        public string CreatedByName { get; set; } = string.Empty;
        public Guid CreatedById { get; set; }
        public bool HasPendingDonations {get; set;}
    }
}
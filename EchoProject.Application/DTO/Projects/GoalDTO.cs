using AutoMapper;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Application.DTO.Projects
{
    [AutoMap(typeof(Goal))]
    public class GoalDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public long Target { get; set; }
        public GoalTypeDTO GoalType { get; set; } = new(); 
    }
}
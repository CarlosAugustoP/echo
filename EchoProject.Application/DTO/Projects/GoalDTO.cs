using AutoMapper;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Application.DTO.Projects
{
    [AutoMap(typeof(Goal))]
    public class GoalDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public long TargetAmount { get; set; }
        public long CurrentAmount { get; set; }
        public GoalTypeDTO GoalType { get; set; } = new(); 
    }
}
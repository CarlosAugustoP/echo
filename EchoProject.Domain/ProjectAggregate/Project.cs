using EchoProject.Domain.Common;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Domain.ProjectAggregate
{
    public class Project : Entity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid ManagerId { get; private set; }
        public virtual User Manager { get; private set; } = null!;
        private readonly List<Goal> _goals = [];
        public SmartContractAddress SmartContractAddress { get; private set; }
        public IReadOnlyCollection<Goal> Goals => _goals.AsReadOnly();
        private Project() { } // EF Core
        public Project(string title, string description, Guid managerId, SmartContractAddress smartContractAddress)
        {
            Id = Guid.NewGuid();
            Title = title.Length < 100 && title.Length > 0 
                ? title 
                : throw new ArgumentException("Title must be between 1 and 100 characters long.");
            Description = (description.Length < 500 && description.Length > 0)  
                ? description
                : throw new ArgumentException("Description cannot exceed 500 characters and must be bigger than 0");
            ManagerId = managerId;
            SmartContractAddress = smartContractAddress;
        }

        public Goal AddGoal(string title, long target, GoalType goalType)
        {
            var goal = new Goal(Id, title, target, goalType);
            _goals.Add(goal);
            return goal;
        }

        public Goal RemoveGoal(Guid goalId)
        {
            var goal = _goals.FirstOrDefault(g => g.Id == goalId);
            if (goal is not null)
            { 
                _goals.Remove(goal);
                return goal;
            }
            throw new ArgumentException("Goal not found.");
        }

        public void UpdateDetails(string title, string description)
        {
            Title = title.Length < 100 && title.Length > 0 
                ? title 
                : throw new ArgumentException("Title must be between 1 and 100 characters long.");
            Description = (description.Length < 500 && description.Length > 0)  
                ? description
                : throw new ArgumentException("Description cannot exceed 500 characters and must be bigger than 0");
        }
    }
}
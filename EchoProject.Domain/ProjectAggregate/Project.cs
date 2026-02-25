using EchoProject.Domain.Common;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Domain.Models
{
    public class Project : Entity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid ManagerId { get; private set; }
        private readonly List<Goal> _goals = [];
        public IReadOnlyCollection<Goal> Goals => _goals.AsReadOnly();
        public Project(string title, string description, Guid managerId)
        {
            Id = Guid.NewGuid();
            Title = title.Length < 100 && title.Length > 0 
                ? title 
                : throw new ArgumentException("Title must be between 1 and 100 characters long.");
            Description = (description.Length < 500 && description.Length > 0)  
                ? description
                : throw new ArgumentException("Description cannot exceed 500 characters and must be bigger than 0");
            ManagerId = managerId;
        }

        public void AddGoal(string title, long target, GoalType goalType)
        {
            var goal = new Goal(Id, title, target, goalType);
            _goals.Add(goal);
        }
    }
}
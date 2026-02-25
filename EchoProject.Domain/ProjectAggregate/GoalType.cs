using EchoProject.Domain.Common;

namespace EchoProject.Domain.ProjectAggregate
{
    public class GoalType : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        public GoalType(string name, string description)
        {
            Name = name.Length < 50 && name.Length > 0
                ? name
                : throw new ArgumentException("Name must be between 1 and 50 characters long.");
            Description = description.Length < 200 && description.Length > 0
                ? description
                : throw new ArgumentException("Description must be between 1 and 200 characters long.");
            IsActive = true;
        }
    }
}
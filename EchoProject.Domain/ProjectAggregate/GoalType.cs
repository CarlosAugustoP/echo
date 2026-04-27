using EchoProject.Domain.Common;

namespace EchoProject.Domain.ProjectAggregate
{
    public class GoalType : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        private GoalType() { } // EF Core
        public GoalType(string name, string description)
        {
            Name = name.Length < 50 && name.Length > 0
                ? name
                : throw new ArgumentException("O nome deve ter entre 1 e 50 caracteres.");
            Description = description.Length < 200 && description.Length > 0
                ? description
                : throw new ArgumentException("A descrição deve ter entre 1 e 200 caracteres.");
            IsActive = true;
        }
    }
}

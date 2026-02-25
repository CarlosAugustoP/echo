using EchoProject.Domain.Common;

namespace EchoProject.Domain.Models
{
    public class Goal : Entity
    {
        public Guid ProjectId { get; private set; }
        public int GoalTypeId { get; private set; }
        public string Title { get; private set; }
        public long TargetAmount { get; private set; }
        public long CurrentAmount { get; private set; } = 0;
        private readonly List<Vendor> _vendors = [];
        public IReadOnlyCollection<Vendor> Vendors => _vendors.AsReadOnly();

        internal Goal(Guid projectId, string title, long target, int goalTypeId)
        {
            ProjectId = projectId;
            Title = title.Length < 50 && title.Length > 0 
                ? title 
                : throw new ArgumentException("Title must be between 1 and 50 characters long.");
            TargetAmount = target != 0 
                ? target 
                : throw new ArgumentException("Target amount must be greater than zero.");
            GoalTypeId = goalTypeId;
        }

        public void AssignVendor(Vendor vendor)
        {
            if (!_vendors.Contains(vendor)) _vendors.Add(vendor);
        }
    }
}
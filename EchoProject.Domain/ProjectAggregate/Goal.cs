using EchoProject.Domain.Common;
using EchoProject.Domain.Exception.EchoProject.Domain.Common;
using EchoProject.Domain.VendorAggregate;

namespace EchoProject.Domain.ProjectAggregate
{
    public class Goal : Entity
    {
        public Guid ProjectId { get; private set; }
        public Guid GoalTypeId { get; private set; }
        public virtual GoalType GoalType { get; private set; }  
        public virtual Project Project { get; private set; } = null!;
        public string Title { get; private set; }
        public long TargetAmount { get; private set; }
        public long CurrentAmount { get; private set; } = 0;
        public long? CostPerUnit { get; private set; }
        private readonly List<Vendor> _vendors = [];
        public IReadOnlyCollection<Vendor> Vendors => _vendors.AsReadOnly();

        // Construtor atualizado
        internal Goal(Guid projectId, string title, long target, GoalType goalType)
        {
            ProjectId = projectId;
            GoalType = goalType ?? throw new ArgumentNullException(nameof(goalType));
            GoalTypeId = goalType.Id;

            if (goalType.Name == PresetName.Money && CostPerUnit is not null)
            {
                throw new ArgumentException("Cost per unit cannot be defined for money goals.");
            }
            
            Title = title.Length is > 0 and < 50 
                ? title 
                : throw new ArgumentException("Title must be between 1 and 50 characters long.");
                
            TargetAmount = target > 0 
                ? target 
                : throw new ArgumentException("Target amount must be greater than zero.");
        }

        public bool RequiresVendor() => GoalType.Name != PresetName.Money;
        public bool MoneyPendingOnTrustedVendorLiberation() => GoalType.Name != PresetName.Money;
        private Goal() { } // EF Core

        public void AssignVendor(Vendor vendor)
        {
            if (!RequiresVendor())
                throw new DomainException("Could not set vendor for money goal.");
            
            if (vendor.Status != VendorStatus.Approved)
                throw new DomainException("Somente fornecedores aprovados podem ser vinculados a uma meta.");
                
            if (!_vendors.Contains(vendor)) _vendors.Add(vendor);
        }

        public void RegisterDonation(long amount)
        {
            throw new NotImplementedException();
        }
    }
}
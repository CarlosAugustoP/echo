using EchoProject.Domain.Common;
using EchoProject.Domain.Exception.EchoProject.Domain.Common;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.VendorAggregate;

namespace EchoProject.Domain.DonationAggregate
{
    public class Donation : Entity
    {
        public Guid DonorId { get; private set; }
        public User Donor { get; private set; } = null!;
        public Guid GoalId { get; private set; }
        public Goal Goal { get; private set; } = null!;
        public DonationStatus Status { get; private set; }
        /// <summary>
        /// The amount of an item donated.
        /// For money goals, this is equal to the amount of ETH donated.
        /// For non-money goals, this is equal to the amount of units donated.
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// The actual total cost, in ETH paid by the user for this donation. 
        /// For money goals, this is equal to the amount of ETH donated. 
        /// For non-money goals, this is equal to the cost per unit defined in
        /// the goal multiplied by the amount of units donated. 
        /// </summary>
        public decimal TotalCost { get; private set; }
        public string TransactionHash { get; private set; }
        public string? FundsReleaseHash { get; private set; }
        public Guid? TransferredToVendorId { get; private set; }
        public Vendor? TransferredToVendor { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public virtual ICollection<DonationEvent> Events { get; private set; } = [];

        private Donation() { }

        public Donation(Guid donorId, Goal goal, decimal amount, decimal costPurchase, string transactionHash)
        {
            DonorId = donorId;
            GoalId = goal.Id;
            TransactionHash = transactionHash;
            CreatedAt = DateTime.UtcNow;
            Goal = goal;

            if (goal.IsAchieved)
                throw new DomainException("Cannot donate to a goal that has already been achieved.");

            if (goal.IsMoney())
            {
                // For money goals, the ETH paid (costPurchase) IS the amount donated.
                // We set both to costPurchase to ensure the 'progress' matches the 'payment'.
                Amount = costPurchase;
                TotalCost = costPurchase;
            }
            else
            {
                // For item goals, Amount = Quantity (units) and TotalCost = ETH Value.
                Amount = amount;
                TotalCost = costPurchase;
            }

            if (Amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

            Status = goal.MoneyPendingOnTrustedVendorLiberation()
                ? DonationStatus.TransferredToContract
                : DonationStatus.ImmediateTransferToNGOInContract;

            ValidateIfEnoughPayment();

            // Item goals will call RegisterDonation later inside TransferToVendor().
            if (goal.IsMoney())
            {
                goal.RegisterDonation(Amount);
            }
        }

        public void SetFundsReleasedHash(string fundsReleaseHash)
        {
            FundsReleaseHash = fundsReleaseHash;
        }

        private void ValidateIfEnoughPayment()
        {
            // Only for non-money goals we need to validate the cost purchase against the goal's cost per unit
            if (Goal.GoalType.Name != PresetName.Money)
            {
                var userPaidThisMuch = TotalCost; // How much the user paid as a whole for the donation
                var weNeedThisMuch = Goal.CostPerUnit * Amount; // How much money is required for this donation based on the goal's cost per unit and the amount of units donated

                if (userPaidThisMuch < weNeedThisMuch)
                {
                    throw new DomainException($"The amount paid: {userPaidThisMuch} is less than the cost per unit: {Goal.CostPerUnit} for this goal.");
                }
            }
        }

        public void TransferToVendor(Vendor vendor)
        {
            if (Status != DonationStatus.TransferredToContract)
                throw new DomainException("A doação não está em estado de transferência para fornecedor.");

            if (!Goal.Vendors.Contains(vendor))
                throw new DomainException("O fornecedor não está vinculado à meta desta doação.");

            if (!vendor.IsValid())
                throw new DomainException("O fornecedor não é aprovado para receber a doação.");

            Status = DonationStatus.TransferredToVendorPending;
            TransferredToVendor = vendor;
            TransferredToVendorId = vendor.Id;
            Goal.RegisterDonation(Amount);
        }

        public DonationEvent AddEvent(DonationStatus status)
        {
            var evt = DonationEventFactory.Create(this, status);
            Events.Add(evt);
            return evt;
        }

        public void UpdateStatus(DonationStatus newStatus)
        {
            if (newStatus == DonationStatus.TransferredToVendorConfirmed || newStatus == DonationStatus.ImmediateTransferToNGOConfirmed)
            {
                CompleteTransfer();
            }
            else if (newStatus == DonationStatus.Failed)
            {
                MarkAsFailed();
            }
            else if (newStatus == DonationStatus.ExpiredAndRefunded)
            {
                MarkAsExpiredAndRefunded();
            }
            else
            {
                throw new DomainException("Invalid status update.");
            }
        }

        private void CompleteTransfer()
        {
            Status = Status switch
            {
                DonationStatus.TransferredToVendorPending => DonationStatus.TransferredToVendorConfirmed,
                DonationStatus.ImmediateTransferToNGOInContract => DonationStatus.ImmediateTransferToNGOConfirmed,
                _ => throw new DomainException($"Cannot confirm transfer. Current status: {Status}"),
            };

        }

        private void MarkAsFailed()
        {
            if (Status == DonationStatus.TransferredToVendorConfirmed)
                throw new DomainException("Não é possível marcar como falhada uma doação já transferida para o fornecedor.");

            Status = DonationStatus.Failed;
        }

        private void MarkAsExpiredAndRefunded()
        {
            if (Status == DonationStatus.TransferredToVendorConfirmed)
                throw new DomainException("Não é possível marcar como expirada e reembolsada uma doação já transferida para o fornecedor.");

            Status = DonationStatus.ExpiredAndRefunded;
        }


    }
}

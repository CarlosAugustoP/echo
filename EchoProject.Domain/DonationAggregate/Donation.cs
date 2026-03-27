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

        public decimal Amount { get; private set; }
        public decimal TotalCost { get; private set; }
        public string TransactionHash { get; private set; }
        public string? FundsReleaseHash { get; private set; }
        public Guid? TransferredToVendorId { get; private set; }
        public Vendor? TransferredToVendor { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public virtual ICollection<DonationEvent> Events { get; private set; } = [];

        private Donation() { }

        public Donation(Guid donorId, Goal goal, decimal amount, decimal? costPurchase, string transactionHash)
        {
            DonorId = donorId;
            GoalId = goal.Id;
            Amount = amount > 0 ? amount : throw new ArgumentException("Amount must be greater than zero.");
            TransactionHash = transactionHash;
            CreatedAt = DateTime.UtcNow;
            Goal = goal;
            // 1st scenario: costPurchase is passed (not money) so user paid a total amount of money for the donation. 
            // 2nd scenario: costPurchase is null (money) so we consider the amount of ETH donated as the total cost paid by the user.
            TotalCost = costPurchase ?? amount;
            Status = goal.MoneyPendingOnTrustedVendorLiberation()
                ? DonationStatus.TransferredToContract : DonationStatus.ImmediateTransferToNGOInContract;

            ValidatePayment();
            Events.Add(DonationEventFactory.Create(this, Status));

        }

        public void SetFundsReleasedHash(string fundsReleaseHash)
        {
            FundsReleaseHash = fundsReleaseHash;
        }

        private void ValidatePayment()
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

        public void AddEvent(DonationEvent donationEvent)
        {
            Events.Add(donationEvent);
        }

        public void UpdateStatus(DonationStatus newStatus)
        {
            if (newStatus == DonationStatus.TransferredToVendorConfirmed)
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
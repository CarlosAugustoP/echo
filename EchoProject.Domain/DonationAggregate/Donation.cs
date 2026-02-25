using EchoProject.Domain.Common;
using EchoProject.Domain.Exception.EchoProject.Domain.Common;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.UserAggregate;

namespace EchoProject.Domain.DonationAggregate
{
    public class Donation : Entity
    {
        public Guid DonorId { get; private set; }
        public User Donor { get; private set; } = null!;
        public Guid GoalId { get; private set; }
        public Goal Goal { get; private set; } = null!;
        public DonationStatus Status { get; private set; }
        
        public long Amount { get; private set; }
        public string TransactionHash { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Donation() { }

        public Donation(Guid donorId, Guid goalId, long amount, string txHash)
        {
            if (amount <= 0)
                throw new ArgumentException("O valor da doação deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(txHash))
                throw new ArgumentException("O Hash da transação blockchain é obrigatório.");

            DonorId = donorId;
            GoalId = goalId;
            Amount = amount > 0 ? amount : throw new ArgumentException("Amount must be greater than zero.");
            TransactionHash = txHash;
            CreatedAt = DateTime.UtcNow;
            Status = Goal?.MoneyPendingOnTrustedVendorLiberation() == true
                ? DonationStatus.PendingVendorRepass : DonationStatus.ImmediateTransferToNGO;
        }

        public void TransferToVendor()
        {
            if (Status != DonationStatus.PendingVendorRepass)
                throw new DomainException("A doação não está em estado de transferência para fornecedor.");

            Status = DonationStatus.TransferredToVendor;
        }

        public void MarkAsFailed()
        {
            if (Status == DonationStatus.TransferredToVendor)
                throw new DomainException("Não é possível marcar como falhada uma doação já transferida para o fornecedor.");

            Status = DonationStatus.Failed;
        }

        public void MarkAsExpiredAndRefunded()
        {
            if (Status == DonationStatus.TransferredToVendor)
                throw new DomainException("Não é possível marcar como expirada e reembolsada uma doação já transferida para o fornecedor.");
            
            Status = DonationStatus.ExpiredAndRefunded;
        }

    }
}
using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Application.DTO.Donations
{
    public class DonationDTO
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalCost { get; set; }
        public string TransactionHash { get; set; } = string.Empty;
        public string? FundsReleaseHash { get; set; }
        public DonationStatus Status  { get; set; }
        public string StatusDesc => Status.ToShortFriendlyString();
        public string NameItem { get; set; } = string.Empty;
        public Guid DonorId { get; set; }
        public Guid GoalId { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? TransferredToVendorId { get; set; }
    }
}
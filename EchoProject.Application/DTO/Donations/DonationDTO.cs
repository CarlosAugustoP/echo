namespace EchoProject.Application.DTO.Donations
{
    public class DonationDTO
    {
        public long Amount { get; set; }
        public long TotalCost { get; set; }
        public string TransactionHash { get; set; } = string.Empty;
        public string NameItem { get; set; } = string.Empty;
        public Guid DonorId { get; set; }
        public Guid GoalId { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
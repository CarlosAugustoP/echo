namespace EchoProject.Application.Requests.Donation
{
    public record DonationRequest
    (
        long Amount,
        long? TotalAmount, 
        Guid GoalId
    );
}
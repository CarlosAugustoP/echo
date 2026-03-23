using System.Numerics;

namespace EchoProject.Application.Requests.Donation
{
    public record DonationRequest
    (
        decimal Amount,
        decimal TotalAmount, 
        Guid GoalId,
        string TransactionHash
    );
}
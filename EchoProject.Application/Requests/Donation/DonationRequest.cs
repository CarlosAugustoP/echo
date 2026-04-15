using System.Numerics;

namespace EchoProject.Application.Requests.Donation
{
    public record DonationRequest
    (
        decimal Amount,
        decimal TotalAmountETH, 
        Guid GoalId,
        string TransactionHash
    );
}
using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Application.Events
{
    public record DonationStatusUpdatedMessage(
        Guid DonationId, 
        DonationStatus NewStatus, 
        string TransactionHash
    );
}
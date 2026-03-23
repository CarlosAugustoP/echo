using EchoProject.Domain.DonationAggregate;

namespace EchoProject.BlockchainWorker
{
    public record DonationStatusUpdatedMessage(
        Guid DonationId, 
        DonationStatus NewStatus, 
        string TransactionHash
    );
}
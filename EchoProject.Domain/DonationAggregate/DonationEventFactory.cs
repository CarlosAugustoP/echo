namespace EchoProject.Domain.DonationAggregate
{
    public static class DonationEventFactory
    {
        public static DonationEvent Create(Donation donation, DonationStatus status)
        {
            string message = status switch
            {
                DonationStatus.TransferredToContract => $"Donation transferred to smart contract. Hash: {donation.TransactionHash}",
                DonationStatus.ImmediateTransferToNGOInContract => "Awaiting transfer to NGO.",
                DonationStatus.ImmediateTransferToNGOConfirmed => "Transfer to NGO confirmed on Blockchain.",
                DonationStatus.TransferredToVendorPending => $"Transfer transaction sent to Blockchain. Hash: {donation.TransactionHash}",
                DonationStatus.TransferredToVendorConfirmed => "Transfer successfully confirmed on Blockchain and received by vendor.",
                DonationStatus.Failed => "The transaction failed or was reverted on Blockchain. Check balance or gas.",
                _ => $"Status changed to: {status}"
            };

            return new DonationEvent(donation, status, message);
        }
    }
}
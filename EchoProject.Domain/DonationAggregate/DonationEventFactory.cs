namespace EchoProject.Domain.DonationAggregate
{
    public static class DonationEventFactory
    {
        public static DonationEvent Create(Donation donation, DonationStatus status)
        {
            string message = status switch
            {
                DonationStatus.TransferredToContract => $"Donation transferred to smart contract. Hash: {donation.TransactionHash}",
                DonationStatus.ImmediateTransferToNGOInContract => "Awaiting direct transfer of funds to the NGO from the smart contract.",
                DonationStatus.ImmediateTransferToNGOConfirmed => $"Transfer to NGO confirmed on Blockchain: {donation.FundsReleaseHash}",
                DonationStatus.TransferredToVendorPending => $"Transfer transaction requested by NGO to trusted vendor.",
                DonationStatus.TransferredToVendorConfirmed => $"Transfer successfully confirmed on Blockchain and received by vendor. Hash: {donation.FundsReleaseHash}",
                DonationStatus.Failed => "The transaction failed or was reverted on Blockchain. Check balance or gas.",
                _ => $"Status changed to: {status}"
            };

            return new DonationEvent(donation, status, message);
        }
    }
}
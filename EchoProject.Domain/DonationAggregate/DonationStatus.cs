namespace EchoProject.Domain.DonationAggregate
{
    public enum DonationStatus
    {
        /// <summary>
        /// The donation is in the smart contract and must me manually released to the supplier.
        /// </summary>
        PendingVendorRepass,
        /// <summary>
        /// The donation has been transferred to the NGO's wallet, as it is a money donation directly to the NGO.
        /// </summary>
        ImmediateTransferToNGO,
        /// <summary>
        /// The donation has been transferred to the supplier's wallet, but we are still waiting for blockchain confirmation.
        /// </summary>
        TransferredToVendorPending,
        /// <summary>
        /// The donation has been transferred to the supplier's wallet and the transaction is confirmed on the blockchain.
        /// </summary>
        TransferredToVendorConfirmed,
        /// <summary>
        /// The donation has failed, either because the transaction was reverted on the blockchain or because it was confirmed but the logs don't match the expected vendor wallet and amount (ex: user sent to a different wallet or with a different amount than expected).
        /// </summary>
        Failed,
        /// <summary>
        /// TODO
        /// </summary>
        ExpiredAndRefunded
    }
}
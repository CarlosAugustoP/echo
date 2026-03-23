namespace EchoProject.Domain.DonationAggregate
{
    public enum DonationStatus
    {
        PendingVendorRepass,
        ImmediateTransferToNGO,
        TransferredToVendorPending,
        TransferredToVendorConfirmed,
        Failed,
        ExpiredAndRefunded
    }
}
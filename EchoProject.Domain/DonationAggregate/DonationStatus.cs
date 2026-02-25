namespace EchoProject.Domain.DonationAggregate
{
    public enum DonationStatus
    {
        PendingVendorRepass,
        ImmediateTransferToNGO,
        TransferredToVendor,
        Failed,
        ExpiredAndRefunded
    }
}
namespace EchoProject.Domain.DonationAggregate
{
    public static class DonationStatusExtensions
    {
        public static string ToShortFriendlyString(this DonationStatus status)
        {
            return status switch
            {
                DonationStatus.TransferredToContract => "Awaiting transfer to trusted supplier",
                DonationStatus.TransferredToVendorPending => "Awaiting blockchain confirmation",
                DonationStatus.TransferredToVendorConfirmed => "Transferred to trusted supplier",
                DonationStatus.ImmediateTransferToNGOInContract => "Awaiting transfer to NGO",
                DonationStatus.ImmediateTransferToNGOConfirmed => "Transferred to NGO",
                DonationStatus.Failed => "Failed",
                DonationStatus.ExpiredAndRefunded => "Expired and refunded",
                _ => "Unknown"
            };
        }
    }
}
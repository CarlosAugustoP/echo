namespace EchoProject.Domain.DonationAggregate
{
    public static class DonationStatusExtensions
    {
        public static string ToShortFriendlyString(this DonationStatus status)
        {
            return status switch
            {
                DonationStatus.TransferredToContract => "Aguardando transferência para fornecedor confiável",
                DonationStatus.TransferredToVendorPending => "Aguardando confirmação na blockchain",
                DonationStatus.TransferredToVendorConfirmed => "Transferido para fornecedor confiável",
                DonationStatus.ImmediateTransferToNGOInContract => "Aguardando transferência para ONG",
                DonationStatus.ImmediateTransferToNGOConfirmed => "Transferido para ONG",
                DonationStatus.Failed => "Falhou",
                DonationStatus.ExpiredAndRefunded => "Expirado e reembolsado",
                _ => "Desconhecido"
            };
        }
    }
}
namespace EchoProject.Domain.DonationAggregate
{
    public static class DonationEventFactory
    {
        public static DonationEvent Create(Donation donation, DonationStatus status)
        {
            string message = status switch
            {
                DonationStatus.PendingVendorRepass => "Aguardando repasse para o fornecedor.",
                DonationStatus.TransferredToVendorPending => $"Transação de repasse enviada ao Blockchain. Hash: {donation.TransactionHash}",
                DonationStatus.TransferredToVendorConfirmed => "Repasse confirmado com sucesso no Blockchain e recebido pelo fornecedor.",
                DonationStatus.Failed => "A transação falhou ou foi revertida no Blockchain. Verifique o saldo ou gás.",
                _ => $"Mudança de status para: {status}"
            };

            return new DonationEvent(donation, status, message);
        }
    }
}
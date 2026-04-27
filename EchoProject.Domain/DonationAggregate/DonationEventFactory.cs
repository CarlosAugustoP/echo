namespace EchoProject.Domain.DonationAggregate
{
    public static class DonationEventFactory
    {
        public static DonationEvent Create(Donation donation, DonationStatus status)
        {
            string message = status switch
            {
                DonationStatus.TransferredToContract => $"Doação transferida para o contrato inteligente. Hash: {donation.TransactionHash}",
                DonationStatus.ImmediateTransferToNGOInContract => "Aguardando transferência direta dos fundos para a ONG a partir do contrato inteligente.",
                DonationStatus.ImmediateTransferToNGOConfirmed => $"Transferência para a ONG confirmada na blockchain: {donation.FundsReleaseHash}",
                DonationStatus.TransferredToVendorPending => "Solicitação de transferência enviada pela ONG para o fornecedor confiável.",
                DonationStatus.TransferredToVendorConfirmed => $"Transferência confirmada com sucesso na blockchain e recebida pelo fornecedor. Hash: {donation.FundsReleaseHash}",
                DonationStatus.Failed => "A transação falhou ou foi revertida na blockchain. Verifique o saldo ou o gás.",
                _ => $"Status alterado para: {status}"
            };

            return new DonationEvent(donation, status, message);
        }
    }
}

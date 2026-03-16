namespace EchoProject.Infrastructure.Blockchain.Interfaces
{
    public interface IEthereumService
    {
        Task<long> GetBalanceAsync(string address);

        // Valida se a transação enviada pelo Frontend é real, 
        // se caiu no contrato certo e se tem o valor correto.
        Task<bool> VerifyTransactionAsync(string txHash, string expectedContractAddress, long expectedAmount);

        // Executa a liberação (assinada pela API) do contrato para o fornecedor
        Task<string> ReleaseFundsToSupplierAsync(string projectAddress, string supplierWallet, long amount);
    }
}
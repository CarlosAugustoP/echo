namespace EchoProject.Infrastructure.Blockchain.Interfaces
{
    public interface IEthereumService
    {
        Task<long> GetBalanceAsync(string address);
        Task<string> DonateToProjectContractAsync(string donorWallet, string contractAddress, long amount);
        Task<string> ReleaseFundsToSupplierAsync(string projectAddress, string supplierWallet, decimal amount);
    }
}
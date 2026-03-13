namespace EchoProject.Infrastructure.Blockchain.Interfaces
{
    public interface IEthereumService
    {
        Task<long> GetBalanceAsync(string address);
        Task<string> DonateToProjectContractAsync(string donorWallet, string contractAddress, long amount);
    }
}
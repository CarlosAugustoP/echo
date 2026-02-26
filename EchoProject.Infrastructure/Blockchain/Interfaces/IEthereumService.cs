namespace EchoProject.Infrastructure.Blockchain.Interfaces
{
    public interface IEthereumService
    {
        Task<long> GetBalanceAsync(string address);
    }
}
using EchoProject.Infrastructure.Blockchain.Interfaces;

namespace EchoProject.Application.Services
{
    public class WalletService
    {
        private readonly IEthereumService _ethereumService;

        public WalletService(IEthereumService ethereumService)
        {
            _ethereumService = ethereumService;
        }

        public async Task<long> GetWalletBalanceAsync(string walletAddress)
        {
            return await _ethereumService.GetBalanceAsync(walletAddress);
        }
    }
}
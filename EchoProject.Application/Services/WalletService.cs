using EchoProject.Application.Common;
using EchoProject.Infrastructure.Blockchain.Interfaces;

namespace EchoProject.Application.Services
{
    [AppService]
    public class WalletService(IEthereumService ethereumService)
    {
        private readonly IEthereumService _ethereumService = ethereumService;

        public async Task<decimal> GetWalletBalanceAsync(string walletAddress)
        {
            return await _ethereumService.GetBalanceAsync(walletAddress);
        }
    }
}
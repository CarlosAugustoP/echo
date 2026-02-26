using EchoProject.Domain.Common;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Configuration;
using Nethereum.Web3;

namespace EchoProject.Infrastructure.Blockchain.Impl
{
    public class EthereumService : IEthereumService
    {
        private readonly Web3 _web3;

        public EthereumService(IConfiguration configuration)
        {
            var rpcUrl = configuration["Ethereum:RpcUrl"];
            _web3 = new Web3(rpcUrl);
        }
        public async Task<long> GetBalanceAsync(string address)
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
            return Web3.Convert.FromWei(balance.Value).ToLong();
        }

    }
}
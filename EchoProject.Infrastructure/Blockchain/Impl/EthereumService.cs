using EchoProject.Domain.Common;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace EchoProject.Infrastructure.Blockchain.Impl
{
    public class EthereumService : IEthereumService
    {
        private readonly Web3 _web3;
        private readonly BlockChainSettings _settings;

        public EthereumService(IOptions<BlockChainSettings> settings)
        {
            _settings = settings.Value;
            var account = new Account(_settings.EthereumPrivateKey, _settings.ChainId);
            _web3 = new Web3(account, _settings.RpcUrl);
        }

        public async Task<long> GetBalanceAsync(string address)
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
            return Web3.Convert.FromWei(balance.Value).ToLong();
        }

    }
}
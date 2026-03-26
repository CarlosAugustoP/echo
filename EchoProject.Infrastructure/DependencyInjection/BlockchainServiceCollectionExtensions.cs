using EchoProject.Infrastructure.Blockchain;
using EchoProject.Infrastructure.Blockchain.Impl;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EchoProject.Infrastructure.DependencyInjection
{
    public static class BlockchainServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBlockChain(this IServiceCollection services, IConfiguration conf)
        {
            services.Configure<BlockChainSettings>(conf.GetSection("BlockChainSettings"));
            services.AddScoped<IEthereumService, EthereumService>();
            return services;
        }
    }
}
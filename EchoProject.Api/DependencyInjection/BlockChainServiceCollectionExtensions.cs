using EchoProject.Infrastructure.Blockchain;
using EchoProject.Infrastructure.Blockchain.Impl;
using EchoProject.Infrastructure.Blockchain.Interfaces;

namespace EchoProject.Api.DependencyInjection
{
    public static class BlockChainServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureBlockChain(this IServiceCollection services, IConfiguration conf)
        {
            services.Configure<BlockChainSettings>(conf.GetSection("BlockChainSettings"));
            services.AddScoped<IEthereumService, EthereumService>();
            return services;
        }
    }
}
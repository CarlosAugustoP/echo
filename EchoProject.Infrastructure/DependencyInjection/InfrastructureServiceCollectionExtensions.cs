using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EchoProject.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPostgresDatabase(configuration);
            services.ConfigureBlockChain(configuration);
            services.AddRepositoriesAndUnitOfWork();
        }
    }
}
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Api.DependencyInjection
{
    public static class DatabaseServiceCollectionExtensions 
    {
        public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var con = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<EchoDbContext>
            (
                o => o.UseNpgsql(con, b => b.MigrationsAssembly("EchoProject.Infrastructure"))
            );
            return services;
        }
        
    }
}
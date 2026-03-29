using EchoProject.Domain.Interfaces;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Repositories;
using EchoProject.Infrastructure.Storage;
using EchoProject.Infrastructure.Storage.Client;
using EchoProject.Infrastructure.Storage.Settings;
using EchoProject.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EchoProject.Infrastructure.DependencyInjection
{
    public static class StorageServiceCollectionExtensions
    {
        public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageSettings>(configuration.GetSection("Supabase"));
            services.AddScoped(sp => 
            {
                var settings = sp.GetRequiredService<IOptions<StorageSettings>>().Value;
                return new Supabase.Client(settings.Url, settings.Key);
            });


            services.AddScoped<IStorageClient, SupabaseStorageClient>(sp => 
            {
                var supabase = sp.GetRequiredService<Supabase.Client>();
                var settings = sp.GetRequiredService<IOptions<StorageSettings>>().Value;
                
                return new SupabaseStorageClient(supabase, settings.BucketName);
            });

            return services;
        }
    }
}
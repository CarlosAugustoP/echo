using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using AssemblyRef = EchoProject.Application.AssemblyReference;
using FluentValidation.AspNetCore;
namespace EchoProject.Application.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAppServices(typeof(AssemblyRef).Assembly);
            services.AddValidatorsFromAssemblyContaining<AssemblyRef>();
            services.AddAutoMapper
            (
                cfg =>
                {
                    cfg.LicenseKey = configuration["AutoMapper:LicenseKey"];
                }, typeof(AssemblyRef).Assembly
            );
            services.AddAuth(configuration);
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            // services.AddMessaging(configuration);

            return services;
        }
    }
}
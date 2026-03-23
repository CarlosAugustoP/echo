using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EchoProject.Api.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        public static void AddMessagingInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration["RabbitMqSettings:Host"] ?? "localhost";

                    cfg.Host(host, h =>
                    {
                        h.Username(configuration["RabbitMqSettings:Username"] ?? "guest");
                        h.Password(configuration["RabbitMqSettings:Password"] ?? "guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
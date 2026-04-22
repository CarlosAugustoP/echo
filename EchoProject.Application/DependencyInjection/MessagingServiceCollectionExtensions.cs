using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.RabbitMq;
using Rebus.Routing.TypeBased;
using EchoProject.Application.Consumers; 

namespace EchoProject.Application.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration["RabbitMqSettings:Host"] ?? "localhost";
            var user = configuration["RabbitMqSettings:Username"] ?? "guest";
            var pass = configuration["RabbitMqSettings:Password"] ?? "guest";
            var vHost = configuration["RabbitMqSettings:VirtualHost"] ?? "/";
            var connectionString = $"amqp://{user}:{pass}@{host}/{Uri.EscapeDataString(vHost)}";
            Console.WriteLine("[API] try to connect to "+ connectionString);

            services.AutoRegisterHandlersFromAssemblyOf<DonationStatusUpdatedConsumer>();

            services.AddRebus(configure => configure
                .Logging(l => l.Console()) 
                .Transport(t => t.UseRabbitMq(connectionString, "echo-project-queue"))
                .Routing(r => r.TypeBased().MapAssemblyOf<Events.DonationStatusUpdatedMessage>("echo-project-queue"))
            );
        }
    }
}

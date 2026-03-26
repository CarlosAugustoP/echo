using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using EchoProject.Application.Events;
using System.Threading.Tasks;

namespace EchoProject.Api.DependencyInjection
{
    public static class RebusWebApplicationExtensions
    {
        public static async Task<WebApplication> SubscribeRebusEventsAsync(this WebApplication app)
        {
            var bus = app.Services.GetRequiredService<IBus>();
            
            await bus.Subscribe<DonationStatusUpdatedMessage>();
            
            return app;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EchoProject.Api.Hubs
{
    [Authorize]
    public class NotificationHub : Hub<INotificationClient>
    {
    }
}

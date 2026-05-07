using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Requests.Notifications;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController(NotificationService notificationService) : EchoController
    {
        private readonly NotificationService _notificationService = notificationService;

        [HttpGet]
        [MandatoryUserFilter]
        public IActionResult GetByCurrentUser([FromQuery] PageRequest pageRequest)
        {
            var result = _notificationService.GetByUser(CurrentUser!, pageRequest);
            return Success(result);
        }

        [HttpGet("unread-count")]
        [MandatoryUserFilter]
        public async Task<IActionResult> GetUnreadCount()
        {
            var result = await _notificationService.GetUnreadCountAsync(CurrentUser!);
            return Success(result);
        }

        [HttpPost("read")]
        [MandatoryUserFilter]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkNotificationsAsReadRequest request)
        {
            var result = await _notificationService.MarkAsReadAsync(CurrentUser!, request);
            return Success(result);
        }
    }
}

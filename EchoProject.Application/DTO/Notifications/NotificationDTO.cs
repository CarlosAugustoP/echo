using AutoMapper;
using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Application.DTO.Notifications
{
    [AutoMap(typeof(Notification))]
    public class NotificationDTO
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}

using AutoMapper;
using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Application.DTO.Donations
{
    [AutoMap(typeof(DonationEvent))]
    public class DonationEventDTO
    {
        public DonationStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = string.Empty;
        public string StatusString => Status.ToShortFriendlyString();
        public DonationEventDTO(){}
    }
}
using EchoProject.Domain.Common;

namespace EchoProject.Domain.DonationAggregate
{
    public class DonationEvent : Entity
    {
        public Guid DonationId { get; private set; }
        public Donation Donation { get; private set; }
        public DonationStatus Status { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Message { get; private set; }

        private DonationEvent() {} //EF core
        public DonationEvent(Donation donation, DonationStatus status, string message)
        {
            DonationId = donation.Id;
            Donation = donation;
            Status = status;
            Timestamp = DateTime.UtcNow;
            Message = message;
        }
    }
}
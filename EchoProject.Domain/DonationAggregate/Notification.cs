using EchoProject.Domain.Common;
using EchoProject.Domain.UserAggregate;

namespace EchoProject.Domain.DonationAggregate
{
    public class Notification : Entity
    {
        public string Message { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public Guid SentTo { get; private set; }
        public virtual User SentToUser { get; private set; }

        private Notification() { }

        public Notification(string message, string description, Guid sentTo)
        {
            Message = message;
            Description = description;
            SentTo = sentTo;
        }
    }
}

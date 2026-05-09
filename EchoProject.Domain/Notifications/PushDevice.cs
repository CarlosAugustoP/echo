using EchoProject.Domain.Common;
using EchoProject.Domain.UserAggregate;

namespace EchoProject.Domain.Notifications
{
    public class PushDevice : Entity
    {
        public Guid UserId { get; private set; }
        public virtual User User { get; private set; } = null!;
        public string Token { get; private set; } = string.Empty;
        public string Platform { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? LastUsedAt { get; private set; }
        public bool IsActive { get; private set; } = true;

        private PushDevice() { }

        public PushDevice(Guid userId, string token, string platform)
        {
            UserId = userId;
            Token = token;
            Platform = platform;
        }

        public void Refresh(Guid userId, string platform)
        {
            UserId = userId;
            Platform = platform;
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsUsed()
        {
            LastUsedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

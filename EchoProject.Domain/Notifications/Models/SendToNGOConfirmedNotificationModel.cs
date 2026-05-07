namespace EchoProject.Domain.Notifications.Models
{
    public record SendToNGOConfirmedNotificationModel(
        Guid DonorId,
        Guid NgoId,
        decimal Amount,
        string ProjectName) : INotificationModel;
}

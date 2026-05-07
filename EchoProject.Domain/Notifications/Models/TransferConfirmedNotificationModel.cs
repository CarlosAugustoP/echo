namespace EchoProject.Domain.Notifications.Models
{
    public record TransferConfirmedNotificationModel(
        Guid DonorId,
        Guid NgoId,
        decimal Amount,
        string ProjectName) : INotificationModel;
}

namespace EchoProject.Domain.Notifications.Models
{
    public record TransferConfirmedNotificationModel(
        Guid DonorId,
        Guid NgoId,
        decimal Amount,
        decimal RealAmountEth,
        string ProjectName) : INotificationModel;
}

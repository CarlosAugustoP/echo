namespace EchoProject.Domain.Notifications.Models
{
    public record SendToVendorConfirmedNotificationModel(
        Guid DonorId,
        Guid NgoId,
        decimal Amount,
        string ProjectName,
        string GoalTitle,
        string VendorName) : INotificationModel;
}

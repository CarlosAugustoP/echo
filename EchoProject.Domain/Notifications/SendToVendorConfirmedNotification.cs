using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Domain.Notifications
{
    public class SendToVendorConfirmedNotification : INotification
    {
        private readonly List<Notification> _notifications = [];

        public record SendToVendorConfirmedNotificationModel(
            Guid DonorId,
            Guid NgoId,
            decimal Amount,
            string ProjectName,
            string GoalTitle,
            string VendorName);

        public void Store(object model)
        {
            if (model is not SendToVendorConfirmedNotificationModel vendorModel)
                throw new ArgumentException("Invalid model for SendToVendorConfirmedNotification");

            const string donorMessage = "Sua doação chegou ao fornecedor com sucesso!";
            string donorDescription =
                $"Sua doação de {vendorModel.Amount} para o projeto {vendorModel.ProjectName} foi confirmada na blockchain e enviada ao fornecedor {vendorModel.VendorName} para atender a meta {vendorModel.GoalTitle}.";

            const string ngoMessage = "Transferência para o fornecedor confirmada!";
            string ngoDescription =
                $"A doação de {vendorModel.Amount} do projeto {vendorModel.ProjectName} foi confirmada na blockchain e recebida pelo fornecedor {vendorModel.VendorName} para a meta {vendorModel.GoalTitle}.";

            _notifications.Add(new Notification(donorMessage, donorDescription, vendorModel.DonorId));
            _notifications.Add(new Notification(ngoMessage, ngoDescription, vendorModel.NgoId));
        }

        public List<Notification> GetNotifications()
        {
            return _notifications;
        }
    }
}

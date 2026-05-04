using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Domain.Notifications
{
    public class SendToNGOConfirmedNotification : INotification
    {
        private readonly List<Notification> _notifications = [];

        public record SendToNGOConfirmedNotificationModel(
            Guid DonorId,
            Guid NgoId,
            decimal Amount,
            string ProjectName);

        public void Store(object model)
        {
            if (model is not SendToNGOConfirmedNotificationModel ngoModel)
                throw new ArgumentException("Invalid model for SendToNGOConfirmedNotification");

            const string donorMessage = "Sua doação foi entregue à ONG com sucesso!";
            string donorDescription =
                $"Sua doação de {ngoModel.Amount} para o projeto {ngoModel.ProjectName} foi confirmada na blockchain e recebida pela ONG responsável.";

            const string ngoMessage = "Transferência para a ONG confirmada!";
            string ngoDescription =
                $"A doação de {ngoModel.Amount} para o projeto {ngoModel.ProjectName} foi confirmada na blockchain e já está disponível para a ONG.";

            _notifications.Add(new Notification(donorMessage, donorDescription, ngoModel.DonorId));
            _notifications.Add(new Notification(ngoMessage, ngoDescription, ngoModel.NgoId));
        }

        public List<Notification> GetNotifications()
        {
            return _notifications;
        }
    }
}

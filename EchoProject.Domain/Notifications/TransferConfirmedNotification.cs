
using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Domain.Notifications
{
    public class TransferConfirmedNotification : INotification
    {
        private readonly List<Notification> _notifications = [];

        public record TransferConfirmedNotificationModel(Guid DonorId, Guid RecipientId, decimal Amount, string ProjectName);

        public void Store(object model)
        {
            if (model is not TransferConfirmedNotificationModel transferModel)
                throw new ArgumentException("Invalid model for TransferConfirmedNotification");

            const string donorMessage = "Sua doação foi transferida com sucesso!";
            string donorDescription = $"Agradecemos por sua generosidade. Sua doação de {transferModel.Amount} para o projeto {transferModel.ProjectName} foi transferida para o Smart Contract e está aguardando a transferência para um fornecedor confiável.";          

            _notifications.Add
            (
                new Notification(donorMessage, donorDescription, transferModel.DonorId)   
            );
        }

        public List<Notification> GetNotifications()
        {
            return _notifications;
        }
    }
}
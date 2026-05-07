using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Notifications.Models;

namespace EchoProject.Domain.Notifications
{
    public class SendToNGOConfirmedNotification : NotificationBase<SendToNGOConfirmedNotificationModel>
    {
        public override NotificationType Type => NotificationType.SendToNGOConfirmed;

        protected override List<Notification> CreateCore(SendToNGOConfirmedNotificationModel model)
        {
            const string donorMessage = "Sua doacao foi entregue a ONG com sucesso!";
            string donorDescription =
                $"Sua doacao de {model.Amount} para o projeto {model.ProjectName} foi confirmada na blockchain e recebida pela ONG responsavel.";

            const string ngoMessage = "Transferencia para a ONG confirmada!";
            string ngoDescription =
                $"A doacao de {model.Amount} para o projeto {model.ProjectName} foi confirmada na blockchain e ja esta disponivel para sua carteira.";

            return
            [
                new Notification(donorMessage, donorDescription, model.DonorId),
                new Notification(ngoMessage, ngoDescription, model.NgoId)
            ];
        }
    }
}

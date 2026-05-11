using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Notifications.Models;

namespace EchoProject.Domain.Notifications
{
    public class SendToVendorConfirmedNotification : NotificationBase<SendToVendorConfirmedNotificationModel>
    {
        public override NotificationType Type => NotificationType.SendToVendorConfirmed;

        protected override List<Notification> CreateCore(SendToVendorConfirmedNotificationModel model)
        {
            const string donorMessage = "Sua doacao chegou ao fornecedor com sucesso!";
            string donorDescription =
                $"Sua doacao de {model.Amount.ToString("F2")} para o projeto {model.ProjectName} foi confirmada na blockchain e enviada ao fornecedor {model.VendorName} para atender a meta {model.GoalTitle}.";

            const string ngoMessage = "Transferencia para o fornecedor confirmada!";
            string ngoDescription =
                $"A doacao de {model.Amount.ToString("F2")} do projeto {model.ProjectName} foi confirmada na blockchain e recebida pelo fornecedor {model.VendorName} para a meta {model.GoalTitle}.";

            return
            [
                new Notification(donorMessage, donorDescription, model.DonorId),
                new Notification(ngoMessage, ngoDescription, model.NgoId)
            ];
        }
    }
}

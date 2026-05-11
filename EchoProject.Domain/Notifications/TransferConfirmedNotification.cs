using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Notifications.Models;

namespace EchoProject.Domain.Notifications
{
    public class TransferConfirmedNotification : NotificationBase<TransferConfirmedNotificationModel>
    {
        public override NotificationType Type => NotificationType.TransferConfirmed;

        protected override List<Notification> CreateCore(TransferConfirmedNotificationModel model)
        {
            const string donorMessage = "Sua doacao foi transferida com sucesso!";
            string donorDescription =
                $"Agradecemos por sua generosidade. Sua doacao de {model.RealAmountEth.ToString("F2")} itens para o projeto {model.ProjectName} foi transferida para o Smart Contract e esta aguardando a transferencia para um fornecedor confiavel.";
            const string ngoMessage = "Nova doação recebida!";
            string ngoDescription =
                $"O projeto {model.ProjectName} recebeu uma nova doacao de {model.RealAmountEth.ToString("F2")}. Realiza a transferência para o fornecedor e continue fortalecendo o living ledger!";
            return
            [
                new(donorMessage, donorDescription, model.DonorId),
                new(ngoMessage, ngoDescription, model.NgoId)
            ];
        }
    }
}

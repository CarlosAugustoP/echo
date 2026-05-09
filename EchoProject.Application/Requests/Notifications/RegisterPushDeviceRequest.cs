namespace EchoProject.Application.Requests.Notifications
{
    public record RegisterPushDeviceRequest(string Token, string Platform);
}

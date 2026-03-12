namespace EchoProject.Application.Requests.Project
{
    public record GoalRequest
    (
        string Title,
        long TargetAmount,
        long CurrentAmount,
        List<Guid> VendorIds,
        Guid GoalTypeId
    );
}
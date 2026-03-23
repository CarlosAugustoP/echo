namespace EchoProject.Application.Requests.Projects
{
    public record GoalRequest
    (
        string Title,
        long TargetAmount,
        long CurrentAmount,
        long? CostPerUnit,
        List<Guid>? VendorIds,
        Guid GoalTypeId
    );
}
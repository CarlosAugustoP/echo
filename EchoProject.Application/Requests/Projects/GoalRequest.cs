namespace EchoProject.Application.Requests.Projects
{
    public record GoalRequest
    (
        string Title,
        decimal TargetAmount,
        decimal CurrentAmount,
        decimal? CostPerUnit,
        List<Guid>? VendorIds,
        Guid GoalTypeId
    );
}
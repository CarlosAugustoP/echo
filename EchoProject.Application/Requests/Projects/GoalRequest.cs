namespace EchoProject.Application.Requests.Projects
{
    public record GoalRequest
    (
        string Title,
        string? Description,
        decimal TargetAmount,
        decimal CurrentAmount,
        decimal? CostPerUnit,
        List<Guid>? VendorIds,
        Guid GoalTypeId
    );
}
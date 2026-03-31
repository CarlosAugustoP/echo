namespace EchoProject.Application.DTO.Dashboard
{
    public record ContributionSummaryDTO(decimal TotalContributed, decimal VariationInCurrentMonth)
    {
        public string VariationInCurrentMonthPercentage => VariationInCurrentMonth.ToString("P2") + "%";
    }
}
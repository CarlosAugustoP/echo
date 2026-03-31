namespace EchoProject.Application.DTO.Dashboard
{
    public class ImpactByRegionDTO(string countryCode, string stateCode, decimal amount)
    {
        private readonly string _countryCode = countryCode;
        private readonly string _stateCode = stateCode;
        public decimal Amount { get; init; } = amount;
        public string Region => string.IsNullOrEmpty(_stateCode) ? _countryCode : $"{_countryCode}-{_stateCode}";
    }
}
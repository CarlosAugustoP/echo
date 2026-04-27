using EchoProject.Domain.Common;

namespace EchoProject.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string PostCode { get; set; }
        public string Street { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CountryCode { get; set; }
        public int? Number { get; set; }

        public Address(string postCode, string street, string neighborhood, string city, string state, string countryCode, int? number = null)
         {
            PostCode = ValidatePostCode(postCode);
            Street = street;
            Neighborhood = neighborhood;
            City = city;
            State = ValidateState(state);
            CountryCode = ValidateCountryCode(countryCode);
            Number = number;
         }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return PostCode;
            yield return Street;
            yield return Neighborhood;
            yield return City;
            yield return State;
            yield return CountryCode;
            yield return Number;
        }
        private static string ValidatePostCode(string postCode)
        {
            if (string.IsNullOrWhiteSpace(postCode))
                throw new ArgumentException("O CEP não pode estar vazio.");

            var digits = Helpers.OnlyDigits(postCode);

            if (digits.Length != 8)
                throw new ArgumentException("O CEP deve ter 8 dígitos.");

            return digits;
        }
        public static string ValidateCountryCode(string countryCode)
        {
            if (countryCode.Length != 2)
            {
                throw new ArgumentException("O código do país deve ter exatamente 2 letras.");
            }
            if (countryCode.Any(char.IsDigit) || countryCode.Any(char.IsSymbol))
            {
                throw new ArgumentException("O código do país deve conter apenas letras.");
            }
            return countryCode.ToUpper();
        }
        public static string ValidateState(string state) => ValidateCountryCode(state);

    }
}

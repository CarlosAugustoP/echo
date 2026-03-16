using EchoProject.Domain.Common;

namespace EchoProject.Domain.ValueObjects
{
    public class SmartContractAddress : ValueObject
    {
        public string Value { get; private set; }

        public SmartContractAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Smart contract address cannot be empty.");

            Value = value;
        }


        protected override IEnumerable<string?> GetEqualityComponents()
        {
            yield return Value;
        }
        
        public static implicit operator string(SmartContractAddress address) => address.Value;

    }
}
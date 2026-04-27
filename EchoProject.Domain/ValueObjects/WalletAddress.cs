using EchoProject.Domain.Common;

namespace EchoProject.Domain.ValueObjects
{
    public class WalletAddress : ValueObject
    {
        public string Address { get; }

        public WalletAddress(string? address)
        {
            if (string.IsNullOrEmpty(address) || !address.StartsWith("0x"))
            {
                throw new ArgumentException("Formato de endereço de carteira inválido.", nameof(address));
            }
            Address = address;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Address;
        }

        public static implicit operator string(WalletAddress wallet) => wallet.Address;
        public override string ToString() => Address;

    }
}

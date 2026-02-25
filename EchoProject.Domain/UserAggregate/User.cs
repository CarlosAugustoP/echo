using EchoProject.Domain.Common;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Domain.UserAggregate
{
    public class User : Entity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public TaxId TaxId { get; private set; }
        public UserRole Role { get; private set;}
        public Address Address { get; private set; }
        public WalletAddress WalletAddress { get; private set; }

        public User(string? name, string? email, TaxId taxId, WalletAddress walletAddress, Address address, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");
            
            if (!email.Contains("@") || !email.Contains("."))
                throw new ArgumentException("Email must be valid.");

            Name = name;
            Address = address;
            Role = role;
            Email = email.ToLower();
            TaxId = taxId;
            WalletAddress = walletAddress;
        }

        private User() { } // EF Core

        public void UpdateWalletAddress(WalletAddress newWalletAddress)
        {
            WalletAddress = newWalletAddress;
        }
    }
}
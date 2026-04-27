using System.Text.RegularExpressions;
using EchoProject.Domain.Common;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Domain.UserAggregate
{
    public class User : Entity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public TaxId TaxId { get; private set; }
        public UserRole Role { get; private set;}
        public Address Address { get; private set; }
        public WalletAddress WalletAddress { get; private set; }
        public ImageUrl? ProfilePicture { get; private set; } = null;

        public User(string? name, string? email, string? passwordHash, TaxId taxId, WalletAddress walletAddress, Address address, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome não pode estar vazio.");
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail não pode estar vazio.");
            
            if (!email.Contains("@") || !email.Contains("."))
                throw new ArgumentException("O e-mail deve ser válido.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("A senha não pode estar vazia.");

            if (taxId == null)
                throw new ArgumentException("O documento fiscal não pode ser nulo.");

            if (role == UserRole.NGO && taxId.IsCpf == true)
                throw new ArgumentException("ONGs devem possuir um CNPJ.");

            Name = name;
            Address = address;
            Role = role;
            Email = email.ToLower();
            TaxId = taxId;
            PasswordHash = passwordHash;
            WalletAddress = walletAddress;
        }

        private User() { } // EF Core

        public void UpdateWalletAddress(WalletAddress newWalletAddress)
        {
            WalletAddress = newWalletAddress;
        }

        public void UpdateInformation(string? name, string? email, Address address, ImageUrl? profilePicture)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!email.Contains("@") || !email.Contains("."))
                    throw new ArgumentException("O e-mail deve ser válido.");

                Email = email.ToLower();
            }

            if (address != null)
                Address = address;

            ProfilePicture = profilePicture;
        }

    }
}

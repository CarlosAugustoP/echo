using AutoMapper;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Application.DTO
{
    [AutoMap(typeof(User))]
    public class UserDTO(Guid id, string name, string email, string walletAddress, TaxId taxId, UserRole role)
    {
        public Guid Id { get; set; } = id;  
        public string Name { get; set; } = name;
        public string Email { get; set; } = email;
        public string WalletAddress { get; set; } = walletAddress;
        public TaxId TaxId { get; set; } = taxId;
        public UserRole Role { get; set; } = role;
        public ImageUrl? ProfilePicture { get; set; } = null;
    }
}
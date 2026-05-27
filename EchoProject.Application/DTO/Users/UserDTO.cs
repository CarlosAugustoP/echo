using AutoMapper;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Application.DTO
{
    [AutoMap(typeof(User))]
    public class UserDTO(Guid id, string name, string email, string walletAddress, TaxId taxId, UserRole role, DateTime? verifiedAt = null, string? bio = null, ImageUrl? profilePicture = null)
    {
        public Guid Id { get; set; } = id;  
        public string Name { get; set; } = name;
        public string Email { get; set; } = email;
        public string WalletAddress { get; set; } = walletAddress;
        public TaxId TaxId { get; set; } = taxId;
        public UserRole Role { get; set; } = role;
        public DateTime? VerifiedAt { get; set; } = verifiedAt;
        public bool IsVerified => VerifiedAt != null;
        public string? Bio { get; set; } = bio;
        public ImageUrl? ProfilePicture { get; set; } = profilePicture;
    }
}

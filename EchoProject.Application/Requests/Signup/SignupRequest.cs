using EchoProject.Domain.UserAggregate;

namespace EchoProject.Application.Requests.Signup
{
    public record SignupRequest
    (
        string Name,
        string Email, 
        string Password, 
        string TaxId, 
        string WalletAddress, 
        AddressRequest Address, 
        UserRole Role = UserRole.Donor
    );
    
}
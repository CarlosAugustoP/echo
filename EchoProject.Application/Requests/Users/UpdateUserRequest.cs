using EchoProject.Application.Requests.Signup;

namespace EchoProject.Application.Requests.Users
{
    public record UpdateUserRequest(string? Name, string? Email, string? Bio, AddressRequest? Address, string? ProfilePictureBase64);
}

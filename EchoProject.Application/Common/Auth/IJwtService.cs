using System.Security.Claims;
using EchoProject.Application.DTO;

namespace EchoProject.Application.Common.Auth
{
    public interface IJwtService
    {
        string GenerateToken(UserDTO user);
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }
}
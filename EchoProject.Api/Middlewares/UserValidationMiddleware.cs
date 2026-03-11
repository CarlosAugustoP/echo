using System.Security.Claims;
using EchoProject.Application.DTO;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Api.Middlewares
{
    public class UserValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userName = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
                var userEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                var userRoleStr = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

                var wallet = user.FindFirst("walletAddress")?.Value ?? string.Empty;
                var taxId = user.FindFirst("taxId")?.Value ?? string.Empty;

                if (Enum.TryParse<UserRole>(userRoleStr, true, out var roleEnum))
                {
                    var userDto = new UserDTO(userName, userEmail, wallet, new TaxId(taxId), roleEnum);
                    context.Items["User"] = userDto;
                }
            }

            await _next(context);
        }
    }
}
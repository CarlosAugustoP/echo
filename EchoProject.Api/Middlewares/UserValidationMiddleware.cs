using System.Security.Claims;
using System.Globalization;
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
                var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString();
                var wallet = user.FindFirst("walletAddress")?.Value ?? string.Empty;
                var taxId = user.FindFirst("taxId")?.Value ?? string.Empty;
                var isFirstAccessValue = user.FindFirst("isFirstAccess")?.Value;
                var verifiedAtValue = user.FindFirst("verifiedAt")?.Value;
                var bio = user.FindFirst("bio")?.Value;
                var profilePicture = user.FindFirst("profilePicture")?.Value;
                
                if (Enum.TryParse<UserRole>(userRoleStr, true, out var roleEnum))
                {
                    var isFirstAccess = true;
                    if (!string.IsNullOrWhiteSpace(isFirstAccessValue) &&
                        bool.TryParse(isFirstAccessValue, out var parsedIsFirstAccess))
                    {
                        isFirstAccess = parsedIsFirstAccess;
                    }

                    DateTime? verifiedAt = null;
                    if (!string.IsNullOrWhiteSpace(verifiedAtValue) &&
                        DateTime.TryParse(verifiedAtValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsedVerifiedAt))
                    {
                        verifiedAt = parsedVerifiedAt;
                    }

                    ImageUrl? img;
                    if (string.IsNullOrEmpty(profilePicture))
                    {
                        img = null;
                    }
                    else
                    {
                        img = new ImageUrl(profilePicture);
                    }
                    var userDto = new UserDTO(Guid.Parse(id), userName, userEmail, wallet, new TaxId(taxId), roleEnum, isFirstAccess, verifiedAt, string.IsNullOrWhiteSpace(bio) ? null : bio, img);
                    context.Items["User"] = userDto;
                }
            }

            await _next(context);
        }
    }
}

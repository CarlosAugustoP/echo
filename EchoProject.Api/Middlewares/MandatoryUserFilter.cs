using EchoProject.Api.Common;
using EchoProject.Application.DTO;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EchoProject.Api.Middlewares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MandatoryUserFilter(UserRole[]? mandatoryRoles = null) : Attribute, IActionFilter
    {
        private readonly UserRole[] roles = mandatoryRoles 
            ?? [UserRole.Donor, UserRole.NGO, UserRole.EchoAdmin];
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.Items["User"] as UserDTO;

            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult(ApiResult<string?>
                    .Failure("No user is logged in", "UNAUTHORIZED"));
            } 
            else if (!roles.Contains(user.Role))
            {
                context.Result = new ObjectResult(ApiResult<string?>
                    .Failure("User does not have the required role to access this resource", "FORBIDDEN"))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }

    }
}
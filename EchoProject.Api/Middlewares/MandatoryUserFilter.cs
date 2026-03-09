using EchoProject.Application.DTO;
using EchoProject.Application.Exception;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EchoProject.Api.Middlewares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MandatoryUserFilter(UserRole[] mandatoryRoles) : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.Items["User"] as UserDTO;

            if (user == null || !mandatoryRoles.Contains(user.Role))
            {
                throw new UnauthorizedException("User cannot access this resource", "UNAUTHORIZED"); 
            } 
        }

    }
}
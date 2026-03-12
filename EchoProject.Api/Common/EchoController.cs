using EchoProject.Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Common
{
    public class EchoController : ControllerBase
    {
        protected UserDTO? CurrentUser => HttpContext.Items["User"] as UserDTO;
        public IActionResult Success<T>(T data) => Ok(ApiResult<T>.Ok(data));
        public IActionResult CreatedAtAction<T>(string actionName, object routeValues, T data) => CreatedAtAction(actionName, routeValues, ApiResult<T>.Ok(data));
    }
}
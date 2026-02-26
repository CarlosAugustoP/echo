using EchoProject.Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Common
{
    public class EchoController : ControllerBase
    {
        public UserDTO? CurrentUser => HttpContext.Items["CurrentUser"] as UserDTO;
        public IActionResult Success<T>(T data) => Ok(ApiResult<T>.Ok(data));
    }
}
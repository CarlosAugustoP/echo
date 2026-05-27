using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    public class AdminUserController(UserService userService) : EchoController
    {
        private readonly UserService _userService = userService;

        /// <summary>
        /// Marks a user as verified.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("{userId}/verify")]
        [MandatoryUserFilter([UserRole.EchoAdmin])]
        public async Task<IActionResult> VerifyUser([FromRoute] Guid userId)
        {
            var user = await _userService.VerifyUserAsync(userId);
            return Success(user);
        }
    }
}

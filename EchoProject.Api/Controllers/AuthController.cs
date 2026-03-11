using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Requests.Login;
using EchoProject.Application.Requests.Signup;
using EchoProject.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(UserService authService) : EchoController
    {
        private readonly UserService _authService = authService;

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var result = await _authService.RegisterUserAsync(request);
            return Success(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Success(result);
        }

        [HttpPost("me")]
        [Authorize]
        [MandatoryUserFilter]
        public IActionResult Me()
        {
            return Success(CurrentUser);
        }
    }
}
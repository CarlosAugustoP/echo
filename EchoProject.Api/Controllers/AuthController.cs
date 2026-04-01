using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Requests.Login;
using EchoProject.Application.Requests.Signup;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(UserService authService) : EchoController
    {
        private readonly UserService _authService = authService;

        /// <summary>
        /// Register a new user (donor or NGO). The user role is determined by the "type" route parameter (1 for donor, 2 for NGO).
        /// </summary>
        /// <param name="request"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var result = await _authService.RegisterUserAsync(request);
            return Success(result);
        }

        /// <summary>
        /// Authenticate user and return JWT token if successful.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Success(result);
        }

        /// <summary>
        /// Get current authenticated user's profile.
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        [Authorize]
        [MandatoryUserFilter]
        public IActionResult Me()
        {
            return Success(CurrentUser);
        }
    }
}
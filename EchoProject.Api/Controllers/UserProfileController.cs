using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Requests.Users;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    public class UserProfileController : EchoController
    {
        private readonly UserService _userService;

        public UserProfileController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("echo-amount")]
        [MandatoryUserFilter([UserRole.Donor, UserRole.NGO])]
        public IActionResult GetEchoAmount()
        {
            var result = _userService.GetEchos(CurrentUser!);
            return Success(new { echoAmount = result });
        }

        [HttpPatch]
        [MandatoryUserFilter([UserRole.Donor, UserRole.NGO])]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            var updatedUser = await _userService.UpdateProfile(request, CurrentUser!);
            return Success(updatedUser);
        }

        [HttpPatch("wallet-address")]
        [MandatoryUserFilter([UserRole.Donor, UserRole.NGO])]
        public async Task<IActionResult> UpdateWalletAddress([FromBody] UpdateWalletAddressRequest request)
        {
            var user = await _userService.UpdateWalletAddress(CurrentUser!.Id, request.WalletAddress);
            return Success(user);
        }
    }
}
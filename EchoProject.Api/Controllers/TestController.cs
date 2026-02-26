using EchoProject.Api.Common;
using EchoProject.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController(WalletService walletService) : EchoController
    {
        private readonly WalletService _walletService = walletService;
        
        [HttpGet("balance/{walletAddress}")]
        public async Task<IActionResult> GetBalance([FromRoute] string walletAddress)
        {
            var balance = await _walletService.GetWalletBalanceAsync(walletAddress);
            return Success(balance);
        }
    }
}
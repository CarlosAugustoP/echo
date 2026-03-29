using EchoProject.Api.Common;
using EchoProject.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    public class WalletController(WalletService walletService) : EchoController
    {
        private readonly WalletService _walletService = walletService;
        
        /// <summary>
        /// Get the balance of a wallet address. 
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        [HttpGet("balance/{walletAddress}")]
        public async Task<IActionResult> GetBalance([FromRoute] string walletAddress)
        {
            var balance = await _walletService.GetWalletBalanceAsync(walletAddress);
            return Success(balance);
        }
    }
}
using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Requests.Vendor;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    public class VendorController(VendorService vendorService) : EchoController
    {
        private readonly VendorService _vendorService = vendorService;   
        
        [HttpPost("approve/{vendorId}")]
        [MandatoryUserFilter([UserRole.EchoAdmin])]
        public async Task<IActionResult> ApproveVendor([FromRoute] Guid vendorId)
        {
            var result = await _vendorService.ApproveVendorAsync(vendorId, CurrentUser!);
            return Success(result);
        }

        [HttpPost("deny/{vendorId}")]
        [MandatoryUserFilter([UserRole.EchoAdmin])]
        public async Task<IActionResult> DenyVendor([FromRoute] Guid vendorId)
        {
            var result = await _vendorService.RejectVendorAsync(vendorId, CurrentUser!);
            return Success(result);
        }

        [HttpPost]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> CreateVendor([FromBody] VendorRequest request)
        {
            var result = await _vendorService.CreateAsync(request);
            return Success(result);
        }

        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetVendor([FromRoute] Guid vendorId)
        {
            var result = await _vendorService.GetByIdAsync(vendorId);
            return Success(result);
        }
    }
}
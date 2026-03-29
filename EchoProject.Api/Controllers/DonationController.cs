using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Requests.Donation;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/donations")]
    public class DonationController(DonationService donationService) : EchoController
    {
        private readonly DonationService _donationService = donationService;

        [HttpGet("project/{projectId}")]
        [MandatoryUserFilter([UserRole.NGO, UserRole.EchoAdmin])]
        public async Task<IActionResult> GetByProject(Guid projectId, [FromQuery] PageRequest pr)
        {
            var result = await _donationService.FindByProjectAsync(projectId, pr, CurrentUser!);
            return Success(result);
        }

        [HttpPost("donate")]
        [MandatoryUserFilter([UserRole.Donor])]
        public async Task<IActionResult> Donate([FromBody] DonationRequest request)
        {
            var result = await _donationService.DonateAsync(request, CurrentUser!);
            return Success(result);
        }

        [HttpGet("history")]
        [MandatoryUserFilter([UserRole.Donor])]
        public IActionResult GetHistory([FromQuery] PageRequest pr)
        {
            var result = _donationService.GetByDonorId(CurrentUser!.Id, pr);
            return Success(result);
        }

        [HttpGet("view-donation/{id}")]
        [MandatoryUserFilter]
        public async Task<IActionResult> GetDonationById(Guid id)
        {
            var result = await _donationService.GetByIdAsync(id, CurrentUser!);
            return Success(result);
        }

        [HttpPost("transfer-to-vendor/{donationId}/{vendorId}")]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> TransferToVendor(Guid donationId, Guid vendorId)
        {
            var result = await _donationService.AssignDonationToVendorAsync(donationId, vendorId, CurrentUser!);
            return Success(result);
        }

        [HttpGet("timeline/{donationId}")]
        [MandatoryUserFilter([UserRole.Donor])]
        public IActionResult GetDonationHistory(Guid donationId, [FromQuery] PageRequest pr)
        {
            var result = _donationService.GetTimeline(pr, CurrentUser!, donationId);
            return Success(result);
        }

        [HttpGet("donation-distribution")]
        public async Task<IActionResult> GetGlobalDonationDistribution([FromQuery] int topN = 5)
        {
            var result = await _donationService.GetGlobalDonationDistributionPerGoalTypeAsync(topN);
            return Success(result);
        }
    }
}
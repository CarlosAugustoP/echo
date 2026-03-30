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

        /// <summary>
        /// Get donations by project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        [HttpGet("project/{projectId}")]
        [MandatoryUserFilter([UserRole.NGO, UserRole.EchoAdmin])]
        public async Task<IActionResult> GetByProject(Guid projectId, [FromQuery] PageRequest pr)
        {
            var result = await _donationService.FindByProjectAsync(projectId, pr, CurrentUser!);
            return Success(result);
        }

        /// <summary>
        /// Verifies the transaction in ethereum and creates a donation record. Should be called after metamask pop up.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("donate")]
        [MandatoryUserFilter([UserRole.Donor])]
        public async Task<IActionResult> Donate([FromBody] DonationRequest request)
        {
            var result = await _donationService.DonateAsync(request, CurrentUser!);
            return Success(result);
        }

        /// <summary>
        /// Get donation history for the current donor. Only accessible by donor users. Results are paginated and sorted by most recent donations first.
        /// </summary>
        /// <param name="pr"></param>
        /// <returns></returns>
        [HttpGet("history")]
        [MandatoryUserFilter([UserRole.Donor])]
        public IActionResult GetHistory([FromQuery] PageRequest pr)
        {
            var result = _donationService.GetHistoryByDonorId(CurrentUser!.Id, pr);
            return Success(result);
        }

        /// <summary>
        /// Get donation details by donation ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("view-donation/{id}")]
        [MandatoryUserFilter]
        public async Task<IActionResult> GetDonationById(Guid id)
        {
            var result = await _donationService.GetByIdAsync(id, CurrentUser!);
            return Success(result);
        }

        /// <summary>
        /// Assigns a donation to a trusted supplier. This will liberate the transaction to the supplier in the smart contract and confirm the donation.
        /// </summary>
        /// <param name="donationId"></param>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        [HttpPost("transfer-to-vendor/{donationId}/{vendorId}")]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> TransferToVendor(Guid donationId, Guid vendorId)
        {
            var result = await _donationService.AssignDonationToVendorAsync(donationId, vendorId, CurrentUser!);
            return Success(result);
        }

        /// <summary>
        /// Get the timeline of a donation, showing all events (creation, transfer to vendor, etc) with their respective timestamps. 
        /// </summary>
        /// <param name="donationId"></param>
        /// <returns></returns>
        [HttpGet("timeline/{donationId}")]
        [MandatoryUserFilter([UserRole.Donor])]
        public IActionResult GetDonationHistory(Guid donationId)
        {
            var result = _donationService.GetTimeline(CurrentUser!, donationId);
            return Success(result);
        }

        /// <summary>
        /// Get global donation distribution per goal type. Returns the total amount donated for each goal type
        /// </summary>
        [HttpGet("donation-distribution")]
        public async Task<IActionResult> GetGlobalDonationDistribution([FromQuery] int topN = 5)
        {
            var result = await _donationService.GetGlobalDonationDistributionPerGoalTypeAsync(topN);
            return Success(result);
        }

      
    }
}
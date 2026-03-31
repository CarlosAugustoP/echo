using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController(DashboardService dashboardService) : EchoController
    {
        private readonly DashboardService _dashboardService = dashboardService;
     
        [HttpGet("contribution-summary")]
        [MandatoryUserFilter([UserRole.Donor])]
        public IActionResult GetContributionSummary()
        {
            var summary = _dashboardService.GetContributionSummary(CurrentUser!);
            return Success(summary);
        }

        [HttpGet("amount-by-goal-type")]
        [MandatoryUserFilter([UserRole.Donor])]
        public async Task<IActionResult> GetAmountByGoalType()
        {
            var amount = await _dashboardService.GetAmountAndGoalTypeAsync(CurrentUser!);
            return Success(amount);
        }

        [HttpGet("impact-by-region")]
        [MandatoryUserFilter([UserRole.Donor])]
        public async Task<IActionResult> GetImpactByRegion()
        {
            var impact = await _dashboardService.GetImpactByRegionAsync(CurrentUser!.Id);
            return Success(impact);
        }

        [HttpGet("donation-events")]
        [MandatoryUserFilter([UserRole.Donor])]
        public IActionResult GetDonationEvents()
        {
            var events = _dashboardService.GetDonationEvents(CurrentUser!);
            return Success(events);
        }

        [HttpGet("count-by-user")]
        [MandatoryUserFilter([UserRole.Donor])]
        public IActionResult GetCountByUser()
        {
            var result = _dashboardService.GetTotalDonationsCountByUserId(CurrentUser!.Id);
            return Success(result);
        }
    }
}
using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO.Vendor;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Application.Requests.Vendor;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/vendors")]
    public class VendorController(VendorService vendorService) : EchoController
    {
        private readonly VendorService _vendorService = vendorService;   
        
        /// <summary>
        /// Approve a vendor application. Changes the vendor's status to approved, allowing them to be assigned to goals and receive donations.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        [HttpPost("approve/{vendorId}")]
        [MandatoryUserFilter([UserRole.EchoAdmin])]
        public async Task<IActionResult> ApproveVendor([FromRoute] Guid vendorId)
        {
            var result = await _vendorService.ApproveVendorAsync(vendorId, CurrentUser!);
            return Success(result);
        }

        /// <summary>
        /// Reject a vendor application. Changes the vendor's status to rejected, preventing them from being assigned to goals or receiving donations. This action is irreversible. 
        /// <param name="vendorId"></param>
        /// <returns></returns>
        [HttpPost("deny/{vendorId}")]
        [MandatoryUserFilter([UserRole.EchoAdmin])]
        public async Task<IActionResult> DenyVendor([FromRoute] Guid vendorId)
        {
            var result = await _vendorService.RejectVendorAsync(vendorId, CurrentUser!);
            return Success(result);
        }

        /// <summary>
        /// Create a new vendor application. The vendor's status is set to pending by default and must be approved by an EchoAdmin before they can be assigned to goals or receive donations. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> CreateVendor([FromBody] VendorRequest request)
        {
            var result = await _vendorService.CreateAsync(request);
            return Success(result);
        }

        [HttpGet]
        [MandatoryUserFilter]
        public PaginatedList<VendorDTO> GetVendors([FromQuery] PageRequest p)
        {
            return _vendorService.GetAll(p);
        }

        /// <summary>
        /// Get all vendors associated with a goal.
        /// </summary>
        /// <param name="goalId"></param>
        /// <returns></returns>
        [HttpGet("by-goal/{goalId}")]
        [MandatoryUserFilter]
        public async Task<IActionResult> GetVendorsByGoal([FromRoute] Guid goalId)
        {
            var result = await _vendorService.GetVendorsByGoal(goalId);
            return Success(result);
        }

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetVendor([FromRoute] Guid vendorId)
        {
            var result = await _vendorService.GetByIdAsync(vendorId);
            return Success(result);
        }

        /// <summary>
        /// Assign a vendor to a goal.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="goalId"></param>
        /// <returns></returns>
        [HttpPost("vendor/{vendorId}/goal/{goalId}")]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> AssignVendorToGoal([FromRoute] Guid vendorId, [FromRoute] Guid goalId)
        {
            var result = await _vendorService.AssignVendorToGoalAsync(vendorId, goalId, CurrentUser!);
            return Success(result);
        }

        [HttpGet("search")]
        public IActionResult SearchVendors([FromQuery] PageRequest p, [FromQuery] string? search)
        {
            return Success(_vendorService.GetAll(p, search));
        }
    }
}
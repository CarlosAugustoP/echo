using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Common;
using EchoProject.Application.Requests.GoalType;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/goals")]
    public class GoalController(GoalService service) : EchoController
    {
        private readonly GoalService _service = service;

        /// <summary>
        /// Creates a new goal type.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("types")]
        [MandatoryUserFilter([UserRole.EchoAdmin])]
        public async Task<IActionResult> CreateGoalType([FromBody] GoalTypeRequest req)
        {
            var goalType = await _service.CreateGoalType(req);
            return Success(goalType);
        }

        /// <summary>
        /// Gets all goal types
        /// </summary>
        /// <param name="pr"></param>
        /// <returns></returns>
        [HttpGet("types")]
        public IActionResult GetGoalTypes([FromQuery] PageRequest pr)
        {
            var goalType = _service.GetGoalTypes(pr.PageNumber, pr.PageSize);
            return Success(goalType);
        }

    }
}
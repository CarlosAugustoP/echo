using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Application.Requests.Projects;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController(ProjectService service) : EchoController
    {
        private readonly ProjectService _service = service;

        [HttpGet("manager/{managerId}")]
        [Authorize]
        [MandatoryUserFilter]
        public IActionResult GetByManager([FromRoute] Guid managerId, [FromQuery] PageRequest pageRequest)
        {
            var projects = _service.GetByNGO(managerId, pageRequest.PageNumber, pageRequest.PageSize);
            return Success(projects);
        }

        [HttpGet("{id}")]
        [Authorize]
        [MandatoryUserFilter]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var project = await _service.GetByIdAsync(id);
            return Success(project);
        }

        [HttpPost]
        [Authorize]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
        {
            var project = await _service.CreateAsync(request, CurrentUser!);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        [Authorize]
        [MandatoryUserFilter]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request)
        {
            var project = await _service.UpdateAsync(id, request, CurrentUser!);
            return Success(project);
        }

        [HttpPost("{id}/goals")]
        [Authorize]
        [MandatoryUserFilter]
        public async Task<IActionResult> AddGoal(Guid id, [FromBody] GoalRequest request)
        {
            var goal = await _service.AddGoalAsync(id, request, CurrentUser!);
            return CreatedAtAction(nameof(GetById), new { id = goal.Id }, goal);
        }

        [HttpDelete("{id}/goals/{goalId}")]
        [Authorize]
        [MandatoryUserFilter]
        public async Task<IActionResult> RemoveGoal(Guid id, Guid goalId)
        {
            await _service.RemoveGoalAsync(id, goalId, CurrentUser!);
            return NoContent();
        }
    }
}
using EchoProject.Api.Common;
using EchoProject.Api.Middlewares;
using EchoProject.Application.Requests.Docs;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Application.Requests.Projects;
using EchoProject.Application.Services;
using EchoProject.Domain.UserAggregate;
using Microsoft.AspNetCore.Mvc;

namespace EchoProject.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController(ProjectService service) : EchoController
    {
        private readonly ProjectService _service = service;

        /// <summary>
        /// Get projects by NGO (manager) ID with pagination. Only accessible by authenticated users.
        /// </summary>
        /// <param name="managerId"></param>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        [HttpGet("manager/{managerId}")]
        [MandatoryUserFilter]
        public IActionResult GetByManager([FromRoute] Guid managerId, [FromQuery] PageRequest pageRequest)
        {
            var projects = _service.GetByNGO(managerId, pageRequest.PageNumber, pageRequest.PageSize);
            return Success(projects);
        }

        /// <summary>
        /// Get project by ID. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var project = await _service.GetByIdAsync(id);
            return Success(project);
        }

        /// <summary>
        /// Create a new project. Only accessible by authenticated NGO users. The project manager is set to the current user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
        {
            var project = await _service.CreateAsync(request, CurrentUser!);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        /// <summary>
        /// Update project details (title and description). Only accessible by the project manager (NGO user who created the project).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [MandatoryUserFilter]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request)
        {
            var project = await _service.UpdateAsync(id, request, CurrentUser!);
            return Success(project);
        }

        /// <summary>
        /// Add a goal to the project. Only accessible by the project manager (NGO user who created the project).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns> 
        [HttpPost("{id}/goals")]
        [MandatoryUserFilter]
        public async Task<IActionResult> AddGoal(Guid id, [FromBody] GoalRequest request)
        {
            var goal = await _service.AddGoalAsync(id, request, CurrentUser!);
            return CreatedAtAction(nameof(GetById), new { id = goal.Id }, goal);
        }

        /// <summary>
        /// Remove a goal from the project. Only accessible by the project manager (NGO user who created the project).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="goalId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/goals/{goalId}")]
        [MandatoryUserFilter]
        public async Task<IActionResult> RemoveGoal(Guid id, Guid goalId)
        {
            await _service.RemoveGoalAsync(id, goalId, CurrentUser!);
            return NoContent();
        }

        /// <summary>
        /// Get trending projects based on the total amount of donations received.
        /// </summary>
        /// <param name="pageRequest">Pagination parameters for the trending projects list.</param>
        /// <returns>A paged list of trending projects.</returns>
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingProjects([FromQuery] PageRequest pageRequest)
        {
            var projects = await _service.GetTrendingProjectsAsync(pageRequest);
            return Success(projects);
        }

        /// <summary>
        /// Get personalized project recommendations for the current donor user based on their donation history and preferences.
        /// </summary>
        [HttpGet("for-you")]
        [MandatoryUserFilter([UserRole.Donor])]
        public async Task<IActionResult> GetForYou([FromQuery] PageRequest pageRequest)
        {
            var projects = await _service.GetForYouAsync(CurrentUser!, pageRequest);
            return Success(projects);
        }

        /// <summary>
        /// Add a blog post to the project. Only accessible by the project manager (NGO user who created the project). 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("blog-post/project/{projectId}")]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> AddBlogPost(Guid projectId, [FromBody] CreateBlogPostRequest request)
        {
            var blogPost = await _service.AddBlogPostAsync(projectId, request, CurrentUser!);
            return CreatedAtAction(nameof(GetBlogPost), new { blogPostId = blogPost.Id }, blogPost);
        }

        /// <summary>
        /// Get blog post by ID. Used to view the details of a specific blog post, including its content and associated images.
        /// </summary>
        /// <param name="blogPostId"></param>
        /// <returns></returns>
        [HttpGet("blog-post/{blogPostId}")]
        public async Task<IActionResult> GetBlogPost(Guid blogPostId)
        {
            var blogPost = await _service.GetBlogPostByIdAsync(blogPostId);
            return Success(blogPost);
        }

        /// <summary>
        /// Get all blog posts for a project with pagination.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        [HttpGet("blog-post/project/{projectId}")]
        public IActionResult GetBlogPosts(Guid projectId, [FromQuery] PageRequest pageRequest)
        {
            var blogPosts = _service.GetBlogPostsbyProject(projectId, pageRequest);
            return Success(blogPosts);
        }

        /// <summary>
        /// Adds an image to a blog post.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="blogPostId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("blog-post/{projectId}/{blogPostId}/add-image")]
        [MandatoryUserFilter([UserRole.NGO])]
        public async Task<IActionResult> AddImageToBlogPost(Guid projectId, Guid blogPostId, [FromBody] DocumentRequest request)
        {
            await _service.AddImageToProjectBlogPostAsync(projectId, blogPostId, request, CurrentUser!);
            return NoContent();
        }

        [HttpGet("blog-post/for-you")]
        [MandatoryUserFilter([UserRole.Donor])]
        public async Task<IActionResult> GetBlogPostsForYou([FromQuery] PageRequest pageRequest)
        {
            var blogPosts = await _service.GetBlogPostsForYouAsync(CurrentUser!, pageRequest);
            return Success(blogPosts);
        }
    }
}
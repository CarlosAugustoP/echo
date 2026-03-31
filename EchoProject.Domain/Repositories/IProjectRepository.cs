using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        IQueryable<Project> FindByManager(Guid managerId);
        IQueryable<Project> FindTrendingProjects();
        Task<IQueryable<Project>> FindForYou(Guid userId);
        IQueryable<ProjectBlogPost> FindBlogPostByUserInvolvement(Guid userId);
        IQueryable<ProjectBlogPost> FindAllProjectBlogPosts();
        Task<ProjectBlogPost?> FindProjectBlogPostByIdAsync(Guid blogPostId, CancellationToken ct = default);
        void AddBlogPost(ProjectBlogPost blogPost);
    }
}
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        IQueryable<Project> FindByManager(Guid managerId);
        IQueryable<Project> FindTrendingProjects();
        Task<IQueryable<Project>> FindForYou(Guid userId);
    }
}
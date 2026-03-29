using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IGoalRepository : IRepository<Goal>
    {
        IQueryable<Goal> FindByProjectId(Guid projectId);
    }
}
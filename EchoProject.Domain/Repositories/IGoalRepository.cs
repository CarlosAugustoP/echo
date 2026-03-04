using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IGoalRepository : IRepository<Goal>
    {
        IEnumerable<Goal> FindByProjectId(Guid projectId);
    }
}
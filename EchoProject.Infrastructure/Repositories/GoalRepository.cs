using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;

namespace EchoProject.Infrastructure.Repositories
{
    public class GoalRepository(EchoDbContext context) : EfRepository<Goal>(context), IGoalRepository
    {
        public IEnumerable<Goal> FindByProjectId(Guid projectId)
        {
            return _model.Where(g => g.ProjectId == projectId);
        }

    }
}
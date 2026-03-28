using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class GoalRepository(EchoDbContext context) : EfRepository<Goal>(context), IGoalRepository
    {
        protected override IQueryable<Goal> Query => base.Query
            .Include(x => x.Project)
            .ThenInclude(x => x.Manager)
            .Include(p => p.GoalType);
        public IQueryable<Goal> FindByProjectId(Guid projectId)
        {
            return Query.Where(g => g.ProjectId == projectId);
        }

    }
}
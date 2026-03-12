using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class ProjectRepository(EchoDbContext context) : EfRepository<Project>(context), IProjectRepository
    {
        protected override IQueryable<Project> Query 
            => base.Query.Include(p => p.Goals).ThenInclude(g => g.GoalType);
        
        public IEnumerable<Project> FindByManager(Guid managerId)
        {
            return _model.Where(p => p.ManagerId == managerId);
        }
    }
}
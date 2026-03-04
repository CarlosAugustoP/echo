using EchoProject.Domain.Models;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;

namespace EchoProject.Infrastructure.Repositories
{
    public class ProjectRepository(EchoDbContext context) : EfRepository<Project>(context), IProjectRepository
    {
        public IEnumerable<Project> FindByManager(Guid managerId)
        {
            return _model.Where(p => p.ManagerId == managerId);
        }
    }
}
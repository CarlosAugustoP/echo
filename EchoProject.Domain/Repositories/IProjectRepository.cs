using EchoProject.Domain.Models;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        IEnumerable<Project> FindByManager(Guid managerId);
    }
}
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;

namespace EchoProject.Infrastructure.Repositories
{
    public class GoalTypeRepository(EchoDbContext context) : EfRepository<GoalType>(context), IGoalTypeRepository;
}
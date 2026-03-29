using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IGoalTypeRepository : IRepository<GoalType>
    {
        Task<Dictionary<string, decimal>> GetTrendingGoalTypes(int topN);
    }
}
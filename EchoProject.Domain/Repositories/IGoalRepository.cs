using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Domain.Repositories
{
    public interface IGoalRepository : IRepository<Goal>
    {
        IQueryable<Goal> FindByProjectId(Guid projectId);
        Task<Dictionary<string, decimal>> GetTrendingGoalTypes(int topN);
        Task<GoalType?> FindGoalTypeByIdAsync(Guid goalTypeId, CancellationToken ct = default);
        Task<GoalType?> FindGoalTypeByNameAsync(string name, CancellationToken ct = default);
        void AddGoalType(GoalType goalType);
        IQueryable<GoalType> FindAllGoalTypes();
    }
}
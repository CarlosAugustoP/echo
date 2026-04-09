using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class GoalRepository(EchoDbContext context) :
        EfRepository<Goal>(context), IGoalRepository
    {
        private readonly EchoDbContext _db = context;

        protected override IQueryable<Goal> Query => base.Query
            .Include(x => x.Project)
            .ThenInclude(x => x.Manager)
            .Include(p => p.GoalType);

        public void AddGoalType(GoalType goalType)
        {
            _db.GoalTypes.Add(goalType);
        }

        public IQueryable<GoalType> FindAllGoalTypes()
        {
            return _db.GoalTypes.AsNoTracking();
        }

        public IQueryable<Goal> FindByProjectId(Guid projectId)
        {
            return Query.Where(g => g.ProjectId == projectId);
        }

        public async Task<GoalType?> FindGoalTypeByIdAsync(Guid goalTypeId, CancellationToken ct = default)
        {
            return await _db.GoalTypes.FirstOrDefaultAsync(gt => gt.Id == goalTypeId, ct);
        }

        public async Task<GoalType?> FindGoalTypeByNameAsync(string name, CancellationToken ct = default)
        {
            return await _db.GoalTypes.FirstOrDefaultAsync(gt => gt.Name == name, ct);
        }

        public async Task<Dictionary<string, decimal>> GetTrendingGoalTypes(int topN)
        {
            var globalTotal = await _db.Donations.SumAsync(d => d.Amount);

            if (globalTotal == 0) return [];

            return await _db.Donations
                .GroupBy(d => d.Goal.GoalType.Name)
                .Select(group => new
                {
                    GoalTypeName = group.Key,
                    GoalDescription = group.FirstOrDefault()!.Goal.GoalType.Description,
                    Percentage = group.Sum(d => d.Amount) / globalTotal * 100
                })
                .OrderByDescending(x => x.Percentage)
                .Take(topN)
                .ToDictionaryAsync(
                    x => x.GoalDescription,
                    x => Math.Round(x.Percentage, 2)
                );
        }
    }
}
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EchoProject.Infrastructure.Repositories
{
    public class GoalTypeRepository(EchoDbContext context) : EfRepository<GoalType>(context), IGoalTypeRepository
    {
        private readonly EchoDbContext _db = context;
        public async Task<Dictionary<string, decimal>> GetTrendingGoalTypes(int topN)
        {
            var globalTotal = await _db.Donations.SumAsync(d => d.Amount);

            if (globalTotal == 0) return [];

            return await _db.Donations
                .GroupBy(d => d.Goal.GoalType.Name)
                .Select(group => new
                {
                    GoalTypeName = group.Key,
                    Percentage = group.Sum(d => d.Amount) / globalTotal * 100
                })
                .OrderByDescending(x => x.Percentage)
                .Take(topN) 
                .ToDictionaryAsync(
                    x => x.GoalTypeName,
                    x => Math.Round(x.Percentage, 2) 
                );
        }
    }

}
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

        private readonly EchoDbContext _db = context;
        
        public IQueryable<Project> FindByManager(Guid managerId)
        {
            return Query.Where(p => p.ManagerId == managerId);
        }

        public IQueryable<Project> FindTrendingProjects()
        {
            return _db.Donations
                .GroupBy(d => d.Goal.Project)
                .Select(group => new
                {
                    Project = group.Key,
                    TotalAmount = group.Sum(d => d.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .Select(x => x.Project)
                .AsQueryable();
        }

        public async Task<IQueryable<Project>> FindForYou(Guid userId)
        {
            var userPreferences = await _db.Donations
                .Where(d => d.DonorId == userId)
                .Select(d => d.Goal.GoalTypeId)
                .Distinct()
                .ToListAsync();

            var currentUser = await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.Address.State, u.Address.CountryCode })
                .FirstOrDefaultAsync();

            var projects = _db.Projects
                .Select(p => new
                {
                    Project = p,
                    Score = (userPreferences.Any(id => p.Goals.Any(g => g.GoalTypeId == id)) ? 10 : 0) // +10 se bater com interesse
                        + (p.Manager.Address.State == currentUser!.State ? 5 : 0)                    // +5 se for mesmo estado
                        + (p.Manager.Address.CountryCode == currentUser.CountryCode ? 3 : 0)        // +3 se for mesmo país
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Project.CreatedAt) // Desempate por data de criação
                .Select(x => x.Project)
                .Include(p => p.Goals)
                .Include(p => p.Manager)
                .AsQueryable();

            return projects;
        }
    }
}
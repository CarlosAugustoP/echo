using EchoProject.Domain.Interfaces;
using EchoProject.Domain.Repositories;
using EchoProject.Infrastructure.Repositories;
using EchoProject.Infrastructure.UnitOfWork;

namespace EchoProject.Api.DependencyInjection
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoriesAndUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IGoalTypeRepository, GoalTypeRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IGoalRepository, GoalRepository>();
            services.AddScoped<IDonationRepository, DonationRepository>();
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
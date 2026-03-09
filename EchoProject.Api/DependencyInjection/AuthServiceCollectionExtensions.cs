using EchoProject.Application.Common.Auth;

namespace EchoProject.Api.DependencyInjection
{
    public static class AuthServiceCollectionExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IJwtService, JwtService>();
            return services;
        }
    }
}
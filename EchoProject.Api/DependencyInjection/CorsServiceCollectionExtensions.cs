namespace EchoProject.Api.DependencyInjection
{
    public static class CorsServiceCollectionExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                    
                    builder.WithOrigins("http://localhost:8081");
                });
            });

            return services;
        }
    }
}
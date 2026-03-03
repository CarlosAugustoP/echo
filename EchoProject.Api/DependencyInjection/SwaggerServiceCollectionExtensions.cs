namespace EchoProject.Api.DependencyInjection
{
    public static class SwaggerServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen
            (
                options =>
                options.SwaggerDoc("v1", new()
                {
                    Title = "Echo Project API",
                    Version = "v1",
                    Description = "API do Echo Project"
                })
            );
            return services;
        }
    }
}
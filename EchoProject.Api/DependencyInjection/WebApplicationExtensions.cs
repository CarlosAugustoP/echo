namespace EchoProject.Api.DependencyInjection
{
    public static class WebApplicationExtensions
    {
        public static WebApplication AddSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
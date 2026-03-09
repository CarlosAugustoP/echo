using EchoProject.Api.Middlewares;

namespace EchoProject.Api.DependencyInjection
{
    public static class MiddlewareServiceCollectionExtensions
    {
        public static WebApplication AddMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<UserValidationMiddleware>();
            app.UseMiddleware<ExceptionsCatchingMiddleware>();
            return app;
        }
    }
}
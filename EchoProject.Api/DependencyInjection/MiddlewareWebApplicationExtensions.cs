using EchoProject.Api.Middlewares;

namespace EchoProject.Api.DependencyInjection
{
    public static class MiddlewareServiceCollectionExtensions
    {
        public static WebApplication AddMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<ExceptionsCatchingMiddleware>();
            app.UseMiddleware<UserValidationMiddleware>();
            return app;
        }
    }
}
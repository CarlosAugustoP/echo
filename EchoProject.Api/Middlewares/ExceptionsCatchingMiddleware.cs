using EchoProject.Api.Common;
using EchoProject.Application.Exception;
using EchoProject.Domain.Exception.EchoProject.Domain.Common;

namespace EchoProject.Api.Middlewares
{
    public class ExceptionsCatchingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionsCatchingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    ConflictException => StatusCodes.Status409Conflict,
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status400BadRequest
                };

                var response = ApiResult<string?>
                    .Failure(ex.Message, ex.ErrorCode ?? "UNEXPECTED_FAILURE");

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (ArgumentException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var response = ApiResult<string?>
                    .Failure(ex.Message, "INVALID_ARGUMENT");

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = ApiResult<string?>
                    .Failure("An unexpected error occurred", "INTERNAL_SERVER_ERROR");

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
using EchoProject.Api.Common;
using EchoProject.Application.Exceptions;
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
                context.Response.StatusCode = ex switch
                {
                    ConflictException => StatusCodes.Status409Conflict,
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status400BadRequest
                };

                var sc = context.Response.StatusCode;

                var response = ApiResult<string?>
                    .Failure(ex.Message, ex.ErrorCode ?? GetErrorCodeFromException(sc));

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
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro inesperado: " + ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = ApiResult<string?>
                    .Failure("Ocorreu um erro inesperado.", "INTERNAL_SERVER_ERROR");

                await context.Response.WriteAsJsonAsync(response);
            }
        }
        private static string GetErrorCodeFromException(int sc)
        {
            return sc switch
            {
                StatusCodes.Status409Conflict => "CONFLICT",
                StatusCodes.Status404NotFound => "NOT_FOUND",
                StatusCodes.Status401Unauthorized => "UNAUTHORIZED",
                StatusCodes.Status400BadRequest => "BAD_REQUEST",
                _ => "UNEXPECTED_FAILURE"
            };
        }
    }
}

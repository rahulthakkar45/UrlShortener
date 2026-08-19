using System.Security;
using Microsoft.AspNetCore.Diagnostics;

namespace UrlShortener.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, title, detail) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
                SecurityException => (StatusCodes.Status403Forbidden, "Forbidden", exception.Message),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", exception.Message),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
            };
            if (statusCode > 500)
            {
                _logger.LogError(exception, "An unexpected error occurred while processing {Method} {Path}.", context.Request.Method, context.Request.Path);

            }
            else 
            {
                _logger.LogWarning(exception, "Request failed with the status code {StatusCode} for {Method} {Path}.", statusCode, context.Request.Method, context.Request.Path);
            }
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var problem = new
            {
                status = statusCode,
                title,
                detail
            };

            await context.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}

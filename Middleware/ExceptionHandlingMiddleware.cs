using StudentManagementSystem.DTOs;
using System.Net;
using System.Text.Json;

namespace StudentManagementSystem.Middleware
{
    /// <summary>
    /// Global exception-handling middleware.
    /// Catches all unhandled exceptions, logs them, and returns a
    /// structured JSON error response.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by global middleware. Path: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                InvalidOperationException  => (HttpStatusCode.BadRequest,       exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized,    exception.Message),
                KeyNotFoundException       => (HttpStatusCode.NotFound,         exception.Message),
                ArgumentException          => (HttpStatusCode.BadRequest,       exception.Message),
                _                          => (HttpStatusCode.InternalServerError,
                                               "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.FailResult(message);
            var json     = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    // Extension method for clean registration in Program.cs
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

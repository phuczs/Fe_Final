using System.Diagnostics;
using System.Security.Claims;

namespace MyAOS.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();

            // Extract user ID from JWT claims (if authenticated)
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            var method = context.Request.Method;
            var path = context.Request.Path;
            var statusCode = context.Response.StatusCode;

            // Log securely - no headers or body payloads are recorded (cite: 55)
            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms. User: {UserId}",
                method, path, statusCode, sw.ElapsedMilliseconds, userId);
        }
    }
}
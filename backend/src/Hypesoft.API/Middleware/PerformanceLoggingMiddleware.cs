using System.Diagnostics;

namespace Hypesoft.API.Middleware;

public class PerformanceLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceLoggingMiddleware> _logger;

    public PerformanceLoggingMiddleware(
        RequestDelegate next,
        ILogger<PerformanceLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            var executionTime = stopwatch.ElapsedMilliseconds;
            
            if (executionTime > 500)
            {
                _logger.LogWarning(
                    "Request {Method} {Path} took {ExecutionTime}ms - exceeds 500ms threshold",
                    context.Request.Method,
                    context.Request.Path,
                    executionTime);
            }
            else
            {
                _logger.LogInformation(
                    "Request {Method} {Path} completed in {ExecutionTime}ms",
                    context.Request.Method,
                    context.Request.Path,
                    executionTime);
            }
        }
    }
}

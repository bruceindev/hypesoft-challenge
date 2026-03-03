using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Hypesoft.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    message = "Validation failed",
                    errors = validationException.Errors
                        .Select(error => new
                        {
                            propertyName = error.PropertyName,
                            errorMessage = error.ErrorMessage
                        })
                        .ToList()
                };
                _logger.LogWarning(
                    "Validation failure for {Method} {Path} with {ErrorCount} errors",
                    context.Request.Method,
                    context.Request.Path,
                    validationException.Errors.Count());
                break;
            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    message = exception.Message
                };
                break;
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    message = exception.Message
                };
                break;
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response = new
                {
                    message = exception.Message
                };
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    message = "Unauthorized access"
                };
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    message = "An internal server error occurred"
                };
                _logger.LogError(
                    exception,
                    "Unhandled exception for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}

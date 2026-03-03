namespace Hypesoft.API.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var contentSecurityPolicy = _configuration["SecurityHeaders:ContentSecurityPolicy"]
            ?? "default-src 'self'; frame-ancestors 'none'; object-src 'none'; base-uri 'self';";

        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["Referrer-Policy"] = "no-referrer";
        context.Response.Headers["Content-Security-Policy"] = contentSecurityPolicy;

        await _next(context);
    }
}

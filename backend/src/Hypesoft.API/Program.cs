using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.RateLimiting;
using MediatR;
using FluentValidation;
using Hypesoft.Infrastructure;
using Hypesoft.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Hypesoft.API")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hypesoft API",
        Version = "v1",
        Description = "E-commerce API with Clean Architecture + DDD + CQRS"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(Hypesoft.Application.Products.Commands.CreateProduct.CreateProductCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(
    typeof(Hypesoft.Application.Products.Commands.CreateProduct.CreateProductCommand).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Hypesoft.API.Behaviors.ValidationBehavior<,>));

builder.Services.AddAutoMapper(
    typeof(Hypesoft.Application.Products.Commands.CreateProduct.CreateProductCommand).Assembly);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authority = builder.Configuration["Keycloak:Authority"];
        var publicAuthority = builder.Configuration["Keycloak:PublicAuthority"];
        var audience = builder.Configuration["Keycloak:Audience"];

        options.Authority = authority;
        options.Audience = audience;
        options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("Keycloak:RequireHttpsMetadata");

        var validIssuers = new List<string>();
        if (!string.IsNullOrWhiteSpace(authority))
        {
            validIssuers.Add(authority.TrimEnd('/'));
        }
        if (!string.IsNullOrWhiteSpace(publicAuthority))
        {
            validIssuers.Add(publicAuthority.TrimEnd('/'));
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuers = validIssuers,
            NameClaimType = "preferred_username",
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Authentication");

                logger.LogWarning(
                    context.Exception,
                    "JWT authentication failed for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Authentication");

                logger.LogWarning(
                    "JWT challenge triggered for {Method} {Path}. Error: {Error}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Error);

                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Authentication");

                logger.LogWarning(
                    "JWT forbidden for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                {
                    return Task.CompletedTask;
                }

                if (!string.IsNullOrWhiteSpace(audience))
                {
                    var audienceClaim = identity.FindFirst(JwtRegisteredClaimNames.Aud)?.Value;
                    var authorizedParty = identity.FindFirst("azp")?.Value;
                    var audienceIsValid = string.Equals(audienceClaim, audience, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(authorizedParty, audience, StringComparison.OrdinalIgnoreCase);

                    if (!audienceIsValid)
                    {
                        context.Fail("Invalid audience");
                        return Task.CompletedTask;
                    }
                }

                var realmAccessClaim = identity.FindFirst("realm_access")?.Value;
                if (string.IsNullOrWhiteSpace(realmAccessClaim))
                {
                    return Task.CompletedTask;
                }

                using var document = JsonDocument.Parse(realmAccessClaim);
                if (!document.RootElement.TryGetProperty("roles", out var rolesElement) || rolesElement.ValueKind != JsonValueKind.Array)
                {
                    return Task.CompletedTask;
                }

                foreach (var roleElement in rolesElement.EnumerateArray())
                {
                    var role = roleElement.GetString();

                    if (!string.IsNullOrWhiteSpace(role) && !identity.HasClaim(ClaimTypes.Role, role))
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("Manager", policy => policy.RequireRole("admin", "manager"));
    options.AddPolicy("User", policy => policy.RequireRole("admin", "manager", "user"));
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue<int?>("RateLimiting:PermitLimit") ?? 100,
                Window = TimeSpan.FromMinutes(builder.Configuration.GetValue<int?>("RateLimiting:WindowMinutes") ?? 1),
                QueueLimit = builder.Configuration.GetValue<int?>("RateLimiting:QueueLimit") ?? 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests. Please try again later."
        }, cancellationToken);
    };
});

builder.Services.AddHealthChecks()
    .AddMongoDb(
        sp => sp.GetRequiredService<MongoDB.Driver.IMongoClient>(),
        name: "mongodb")
    .AddRedis(builder.Configuration["Redis:Configuration"]!, name: "redis")
    .AddUrlGroup(
        new Uri($"{builder.Configuration["Keycloak:PublicAuthority"]?.TrimEnd('/') ?? builder.Configuration["Keycloak:Authority"]?.TrimEnd('/')}/.well-known/openid-configuration"),
        name: "keycloak-oidc",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "oidc", "keycloak" });

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendOnly", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        policy.WithOrigins(origins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress);
    };
});

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<PerformanceLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hypesoft API v1");
    });
}

app.UseResponseCompression();
app.UseCors("FrontendOnly");
app.UseRateLimiter();

if (builder.Configuration.GetValue<bool?>("HttpsRedirection:Enabled") ?? false)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    AllowCachingResponses = false,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
                error = entry.Value.Exception?.Message
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}).AllowAnonymous();

try
{
    Log.Information("Starting Hypesoft API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
}

namespace Hypesoft.API.Configuration;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}

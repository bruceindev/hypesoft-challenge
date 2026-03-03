using Hypesoft.Domain.Interfaces;
using Hypesoft.Infrastructure.Caching;
using Hypesoft.Infrastructure.Persistence;
using Hypesoft.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("MongoDb")
            ?? throw new InvalidOperationException("Connection string 'MongoDb' was not found.");

        var mongoUrl = MongoUrl.Create(mongoConnectionString);
        var databaseName = mongoUrl.DatabaseName
            ?? configuration["Mongo:DatabaseName"]
            ?? "hypesoft";

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
            options.UseMongoDB(mongoClient, databaseName);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:Configuration"];
            options.InstanceName = configuration["Redis:InstanceName"];
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}

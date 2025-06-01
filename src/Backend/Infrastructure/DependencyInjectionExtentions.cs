using Application.Abstractions;
using Infrastructure.DataContext;
using Infrastructure.Implementation;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Infrastructure;

public static class DependencyInjectionExtentions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepositoryImpl>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasherImpl>();
        services.AddScoped<ITokenGenerator, TokenGeneratorImpl>();

        return services;
    }

    public static IServiceCollection AddMongoDb(this IServiceCollection services, IOptions<MongoConnectionConfig> connectionConfig)
    {
        services.AddSingleton<IMongoClient, MongoClient>(options =>
        {
            return new MongoClient(connectionConfig.Value.ConnectionString);
        });

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}

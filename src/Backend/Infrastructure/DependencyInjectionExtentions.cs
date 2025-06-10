using Application.Abstractions;
using Infrastructure.DataContext;
using Infrastructure.Implementation;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace Infrastructure;

public static class DependencyInjectionExtentions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepositoryImpl>();
        services.AddScoped<IArticlesRepository, ArticlesRepositoryImpl>();
        services.AddScoped<ITagsRepository, TagsRepositoryImpl>();
        services.AddScoped<IRatingsRepository, RatingsRepositoryImpl>();
        services.AddScoped<IMediaFileService, MediaFileServiceImpl>();
        services.AddScoped<IIntegrationRepository, IntegrationRepositoryImpl>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasherImpl>();
        services.AddScoped<ITokenGenerator, TokenGeneratorImpl>();

        services.AddSingleton<IConnection>(sp =>
        {
            IConfiguration config = sp.GetRequiredService<IConfiguration>();
            string host = config["RabbitMq:Host"] ?? "localhost";
            string password = config["RabbitMq:Pass"] ?? throw new ArgumentNullException(nameof(password));
            string name = config["RabbitMq:Name"] ?? throw new ArgumentNullException(nameof(name));

            var factory = new ConnectionFactory()
            {
                HostName = host,
                Password = password,
                UserName = name,
            };

            return factory.CreateConnection();
        });

        return services;
    }

    public static IServiceCollection AddMongoDb(this IServiceCollection services, MongoConnectionConfig connectionConfig)
    {
        services.AddSingleton<IMongoClient, MongoClient>(options =>
        {
            return new MongoClient(connectionConfig.ConnectionString);
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

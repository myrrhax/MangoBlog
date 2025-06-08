using System.Data;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Telegram.Bot;
using TelegramBot.Api;
using TelegramBot.Context;
using TelegramBot.Handlers;
using TelegramBot.Persistence;
using TelegramBot.Routing;
using TelegramBot.Services;

var hostBuilder = Host.CreateDefaultBuilder(args);

Env.Load();

hostBuilder.ConfigureServices((context, services) =>
{
    string botToken = Environment.GetEnvironmentVariable("BOT_TOKEN")
        ?? "";

    services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(botToken));
    services.AddSingleton<ContextManager>();

    Uri.TryCreate(context.Configuration["API_URL"], UriKind.Absolute, out Uri? apiUrl);
    services.AddHttpClient<ApiService>(options =>
    {
        options.BaseAddress = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
    });
    services.AddScoped<IDbConnection>(options =>
    {
        string connectionString = context.Configuration.GetConnectionString("Default")
            ?? throw new ArgumentNullException(nameof(connectionString));
        return new NpgsqlConnection(connectionString);
    });

    services.AddScoped<UsersService>();

    services.AddSingleton<ApiService>();
    services.AddTransient<CommandStartHandler>();

    services.AddSingleton<Router>();

    services.AddHostedService<BotListenerService>();
});

var host = hostBuilder.Build();

using var scope = host.Services.CreateScope();
UsersService service = scope.ServiceProvider.GetService<UsersService>() ??
    throw new ArgumentNullException(nameof(service));

await service.InitializeTable();
await host.RunAsync();
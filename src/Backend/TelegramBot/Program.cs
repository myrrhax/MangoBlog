using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TelegramBot.Api;
using TelegramBot.Context;
using TelegramBot.Handlers;
using TelegramBot.Routing;
using TelegramBot.Services;

var host = Host.CreateDefaultBuilder(args);

Env.Load();

host.ConfigureServices((context, services) =>
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
    services.AddSingleton<ApiService>();
    services.AddSingleton<CommandStartHandler>();

    services.AddSingleton<Router>();

    services.AddHostedService<BotListenerService>();
});

await host.Build().RunAsync();
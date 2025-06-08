using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using TelegramBot.Context;
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
    services.AddSingleton<Router>();

    services.AddHostedService<BotListenerService>();
});

await host.Build().RunAsync();
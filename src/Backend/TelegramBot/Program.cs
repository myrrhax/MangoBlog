using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args);

host.ConfigureServices((context, services) =>
{
    string botToken = Environment.GetEnvironmentVariable("BOT_TOKEN")
        ?? throw new ArgumentNullException(nameof(botToken));

    services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(botToken));
});

host.Build();

await host.StartAsync();
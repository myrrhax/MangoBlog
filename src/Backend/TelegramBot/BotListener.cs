using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace TelegramBot;

internal class BotListenerService : BackgroundService
{
    private readonly ITelegramBotClient _tgBot;
    private readonly 
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
    }
}

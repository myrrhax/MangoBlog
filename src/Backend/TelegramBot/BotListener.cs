using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Context;

namespace TelegramBot;

internal class BotListenerService : BackgroundService
{
    private readonly ITelegramBotClient _tgBot;
    private readonly ContextManager _ctxManager;
    private readonly ILogger<BotListenerService> _logger;

    public BotListenerService(ITelegramBotClient tgBot,
        ContextManager ctxManager, 
        ILogger<BotListenerService> logger)
    {
        _tgBot = tgBot;
        _ctxManager = ctxManager;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _tgBot.StartReceiving(HandleUpdate, 
            HandleError,
            cancellationToken: stoppingToken);

        return Task.CompletedTask;
    }

    private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

    }

    private Task HandleError(ITelegramBotClient telegramBotClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError("An error occurred bot handling: {}", exception.Message);

        return Task.CompletedTask;
    }
}

using System.Collections.Concurrent;
using Telegram.Bot;

namespace TelegramBot.Context;

internal class ContextManager
{
    private readonly ConcurrentDictionary<string, BotContext> _states = new();
    private readonly ITelegramBotClient _telegramBotClient;
    
    public ContextManager(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public bool TryAddContext(string userId, out BotContext? context)
    {
        context = new BotContext(this, _telegramBotClient, userId);
        if (_states.TryAdd(userId, context))
            return true;

        return false;
    }

    public BotContext TryGetOrAddContext(string userId)
    {
        var context = new BotContext(this, _telegramBotClient, userId);
        return _states.GetOrAdd(userId, context);
    }

    public BotContext? GetContext(string userId)
    {
        if (_states.TryGetValue(userId, out BotContext? ctx))
            return ctx;

        return null;
    }

    public bool TryRemoveContext(string userId)
    {
        return _states.TryRemove(userId, out BotContext? _);
    }
}

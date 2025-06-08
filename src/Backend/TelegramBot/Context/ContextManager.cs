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

    public bool TryAddContext(string userId, IState? initialState = null)
    {
        return _states.TryAdd(userId, new BotContext(this, _telegramBotClient, userId, initialState));
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

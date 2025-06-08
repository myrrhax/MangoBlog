﻿using Telegram.Bot;

namespace TelegramBot.Context;

internal class BotContext
{
    private readonly ContextManager _manager;
    private readonly string _userId;
    public Dictionary<string, object> Storage { get; }
    public ITelegramBotClient Bot { get; }
    public IState? CurrentState { get; private set; }

    public BotContext(ContextManager manager, 
        ITelegramBotClient botClient, 
        string userId, 
        IState? initialState = null)
    {
        Bot = botClient;
        CurrentState = initialState;
        Storage = new Dictionary<string, object>();
        _userId = userId;
        _manager = manager;
    }

    public bool UpdateState(IState newState, string userId)
    {
        if (_userId != userId)
            return false;
        CurrentState = newState;
        return true;
    }

    public bool DropState(string userId)
    {
        if (_userId != userId)
            return false;
        return _manager.TryRemoveContext(userId);
    }
}

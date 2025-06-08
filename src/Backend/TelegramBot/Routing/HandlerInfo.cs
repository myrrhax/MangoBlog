using System.Reflection;
using Telegram.Bot.Types.Enums;
using TelegramBot.Context;

namespace TelegramBot.Routing;

internal class HandlerInfo
{
    public string? Command { get; }
    public Type? State { get; }
    public MethodInfo Handler { get; }
    public object Instance { get; }
    public UpdateType UpdateType { get; }

    public HandlerInfo(string? command, 
        Type? state,
        MethodInfo handler,
        object instance, 
        UpdateType updateType)
    {
        Command = command;
        State = state;
        Handler = handler;
        Instance = instance;
        UpdateType = updateType;
    }
}

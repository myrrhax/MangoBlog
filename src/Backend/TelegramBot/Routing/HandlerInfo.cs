using System.Reflection;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Routing;

internal class HandlerInfo
{
    public string? Command { get; }
    public Enum? State { get; }
    public MethodInfo Handler { get; }
    public Type InstanceType { get; }
    public UpdateType UpdateType { get; }
    public string? Query { get; }

    public HandlerInfo(string? command,
        Enum? state,
        MethodInfo handler,
        Type instanceType,
        UpdateType updateType,
        string? query)
    {
        Command = command;
        State = state;
        Handler = handler;
        InstanceType = instanceType;
        UpdateType = updateType;
        Query = query;
    }
}

using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Context;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Routing;

internal class Router
{
    private readonly ContextManager _contextManager;
    private readonly List<HandlerInfo> _handlers;

    private static readonly Dictionary<Type, UpdateType> UpdateTypeMap = new()
    {
        { typeof(Message), UpdateType.Message },
        { typeof(CallbackQuery), UpdateType.CallbackQuery },
        { typeof(InlineQuery), UpdateType.InlineQuery },
        { typeof(ChosenInlineResult), UpdateType.ChosenInlineResult },
    };

    public Router(ContextManager manager)
    {
        _contextManager = manager;
        InitRoutes();
    }

    private void InitRoutes()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        IEnumerable<Type> handlers = assembly.GetTypes()
            .Where(type => type.IsGenericType 
                && type.GetGenericTypeDefinition() == typeof(IHandler<>));

        foreach (Type handler in handlers)
        {
            MethodInfo? handleMethod = handler.GetMethod("HandleAsync");
            if (handleMethod is null)
                continue;
            Type argType = handler.GetGenericArguments()[0];
            if (!UpdateTypeMap.TryGetValue(argType, out UpdateType updateType))
                continue;
            
            var commandAttribute = (CommandAttribute?) handleMethod
                .GetCustomAttribute(typeof(CommandAttribute), false);
            var stateAttribute = (StateAttribute?) handleMethod
                .GetCustomAttribute(typeof(StateAttribute), false);

            if (commandAttribute is null && stateAttribute is null)
                continue;

            var instance = Activator.CreateInstance(handler);
            var info = new HandlerInfo(commandAttribute?.Command,
                stateAttribute?.StateType,
                handleMethod,
                instance,
                updateType);
        }
    }
}

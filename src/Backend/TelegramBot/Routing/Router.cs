using System.Reflection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Context;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Routing;

internal class Router
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ContextManager _contextManager;
    private readonly ILogger<Router> _logger;
    private readonly List<HandlerInfo> _handlers;

    private static readonly Dictionary<Type, UpdateType> UpdateTypeMap = new()
    {
        { typeof(Message), UpdateType.Message },
        { typeof(CallbackQuery), UpdateType.CallbackQuery },
        { typeof(InlineQuery), UpdateType.InlineQuery },
        { typeof(ChosenInlineResult), UpdateType.ChosenInlineResult },
    };

    public Router(ContextManager manager, ILogger<Router> logger, IServiceProvider serviceProvider)
    {
        _contextManager = manager;
        _logger = logger;
        _handlers = new List<HandlerInfo>();
        _serviceProvider = serviceProvider;
        InitRoutes();
    }

    private void InitRoutes()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        IEnumerable<Type> handlers = assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<>)));

        foreach (Type handler in handlers)
        {
            MethodInfo? handleMethod = handler.GetMethod("HandleAsync");
            if (handleMethod is null)
                continue;

            var interfaceType = handler.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<>));
            Type argType = interfaceType.GetGenericArguments()[0];
            if (!UpdateTypeMap.TryGetValue(argType, out UpdateType updateType))
                continue;
            
            var commandAttribute = (CommandAttribute?) handleMethod
                .GetCustomAttribute(typeof(CommandAttribute), false);
            var stateAttribute = (StateAttribute?) handleMethod
                .GetCustomAttribute(typeof(StateAttribute), false);

            if (commandAttribute is null && stateAttribute is null)
                continue;

            var instance = _serviceProvider.GetService(handler);
            if (instance is null)
                continue;

            var info = new HandlerInfo(commandAttribute?.Command,
                stateAttribute?.StateType,
                handleMethod,
                instance,
                updateType);
            _handlers.Add(info);
        }

        _logger.LogInformation("{} - Added {} handlers", 
            DateTime.Now, 
            _handlers.Count);
    }

    public async Task RouteAsync(Update update, CancellationToken cancellationToken)
    {
        string userId;
        string? command = null;
        object updateArg;
        switch (update.Type)
        {
            case UpdateType.Message:
                Message msg = update.Message!;
                userId = msg.From?.Id.ToString() 
                    ?? msg.SenderChat!.Id.ToString();
                if (msg.Text is not null && msg.Text.StartsWith("/"))
                    command = msg.Text.Split(" ")[0];
                updateArg = msg;
                break;
            default:
                _logger.LogInformation("Update: {} пропущен", update.Id);
                return;
        }

        BotContext ctx = _contextManager.TryGetOrAddContext(userId);

        HandlerInfo? handler = _handlers.FirstOrDefault(handler => handler.Command == command?.ToLower()
            && handler.UpdateType == update.Type
            && handler.State == ctx.CurrentState?.GetType());

        if (handler is null)
            _logger.LogInformation("Не найден подходящий хэндлер. Update: {} пропущен", update.Id);

        
        Task? task = (Task?) handler!.Handler.Invoke(handler!.Instance, [ctx, updateArg, cancellationToken]);
        if (task is null)
            return;

        await task;
        if (ctx.CurrentState is null)
            _contextManager.TryRemoveContext(userId);
    }
}

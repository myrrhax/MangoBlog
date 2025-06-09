using Telegram.Bot;

namespace TelegramBot.Context;

internal class BotContext
{
    private readonly ContextManager _manager;
    public Dictionary<string, object> Storage { get; }
    public ITelegramBotClient Bot { get; }
    public Enum? CurrentState { get; set; }

    public BotContext(ContextManager manager, 
        ITelegramBotClient botClient, 
        string userId, 
        Enum? initialState = null)
    {
        Bot = botClient;
        CurrentState = initialState;
        Storage = new Dictionary<string, object>();
        _manager = manager;
    }
}

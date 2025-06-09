using Telegram.Bot.Types;

namespace TelegramBot.Context;

internal abstract class State
{
    protected BotContext _botContext;
    protected State(BotContext context)
    {
        _botContext = context;        
    }
    
    public abstract void Handle();
}
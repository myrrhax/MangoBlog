namespace TelegramBot.Context.States;

internal class RetryAddBotToChatState : State
{
    public RetryAddBotToChatState(BotContext context)
        : base(context) { }
    
    public override void Handle()
    {
        throw new NotImplementedException();
    }
}

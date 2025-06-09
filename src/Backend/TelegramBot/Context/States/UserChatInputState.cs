namespace TelegramBot.Context.States;

internal class UserChatInputState : State
{
    public UserChatInputState(BotContext botContext)
        : base(botContext) { }

    public override void Handle()
    {
        _botContext.Storage.TryGetValue("chatId", out var chatIdData);
        _botContext.Storage.TryGetValue("userId", out var userIdData);
        if (userIdData is null || userIdData is not string userId)
            throw new ArgumentNullException(nameof(userIdData));

        if (chatIdData is null || chatIdData is not string chatId)
        {
            _botContext.DropState(userId);
            return;
        }

        if (_botContext.Storage["isMember"] is not bool isMember || !isMember)
        {
            _botContext.UpdateState(new RetryAddBotToChatState(_botContext), userId);
            return;
        }
        _botContext.DropState(userId);
    }
}

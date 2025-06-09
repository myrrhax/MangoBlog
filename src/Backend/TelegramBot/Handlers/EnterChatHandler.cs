using Telegram.Bot.Types;
using TelegramBot.Context;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;
using TelegramBot.States;

namespace TelegramBot.Handlers;

internal class EnterChatHandler : IHandler<Message>
{
    [State(typeof(UserChatInputState))]
    public async Task HandleAsync(BotContext context, Message update, CancellationToken cancellationToken)
    {
        long chatId = update.Chat.Id;
    }
}

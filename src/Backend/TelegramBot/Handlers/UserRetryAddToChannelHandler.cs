using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Context;
using TelegramBot.Context.States;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Handlers;

internal class UserRetryAddToChannelHandler : IHandler<Message>
{
    [State(typeof(RetryAddBotToChatState))]
    public async Task HandleAsync(BotContext context, Message update, CancellationToken cancellationToken)
    {
        await context.Bot.SendMessage(update.From.Id, "ПРИВЕТ!");
    }
}

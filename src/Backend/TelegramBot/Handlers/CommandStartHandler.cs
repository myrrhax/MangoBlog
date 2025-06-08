using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Context;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Handlers;

internal class CommandStartHandler : IHandler<Message>
{
    [Command("/start")]
    public async Task HandleAsync(BotContext context, Message update, CancellationToken cancellationToken)
    {
        var bot = context.Bot;
        await bot.SendMessage(update.Chat.Id, "Привет", cancellationToken: cancellationToken);
    }
}

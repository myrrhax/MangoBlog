using Telegram.Bot.Types;
using TelegramBot.Context;

namespace TelegramBot.Routing;

internal interface IHandler<T>
{
    Task HandleAsync(BotContext context, T update, CancellationToken cancellationToken);
}

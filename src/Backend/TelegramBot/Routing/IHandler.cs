using Telegram.Bot.Types;
using TelegramBot.Context;

namespace TelegramBot.Routing;

internal interface IHandler<T> where T : Update
{
    Task HandleAsync(BotContext context, T update);
}

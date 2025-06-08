using Telegram.Bot.Types;

namespace TelegramBot.Context;

internal interface IState
{
    Task Handle(BotContext context, Update update);
}
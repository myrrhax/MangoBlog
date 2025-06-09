using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Context;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;
using TelegramBot.States;
using TelegramBot.Utils;

namespace TelegramBot.Handlers;

internal class AddChannelHandler : IHandler<CallbackQuery>
{
    private const string MessageAnswer = "Для того,чтобы добавить канал для интеграции добавьте меня в свой канал <b>с правами Администратора</b>. После чего перешлите мне сообщение с вашего канала";
    [CallbackQueryData(Keyboards.AddChannelQuery)]
    public async Task HandleAsync(BotContext context, CallbackQuery update, CancellationToken cancellationToken)
    {
        await context.Bot.AnswerCallbackQuery(update.Id);

        await context.Bot.SendMessage(update.From.Id, MessageAnswer, parseMode: ParseMode.Html);
        context.UpdateState(new UserChatInputState(), update.From.Id.ToString());
    }
}

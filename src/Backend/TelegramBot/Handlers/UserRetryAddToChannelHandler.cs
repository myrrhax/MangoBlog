using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Context;
using TelegramBot.Context.States;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Handlers;

internal class UserRetryAddToChannelHandler : IHandler<Message>
{
    [State(typeof(UserAddIntegrationChatState), UserAddIntegrationChatState.RetryAddBotToChat)]
    public async Task HandleAsync(BotContext context, Message update, CancellationToken cancellationToken)
    {
        long userId = update.From!.Id;

        if (update.Text == "stop")
        {
            await context.Bot.SendMessage(userId, "Операция отменена");
            context.CurrentState = null;
            return;
        }

        if (context.Storage["chatId"] is not long chatId)
        {
            await context.Bot.SendMessage(userId, "Ошибка при выполнении бота");
            context.CurrentState = null;
            return;
        }
        var me = await context.Bot.GetMe();

        try
        {
            var membership = await context.Bot.GetChatMember(chatId, me.Id);
            if (membership is null)
            {
                await BotStillNotInTheChannelAnswer(context, userId);
                return;
            }
            await context.Bot.SendMessage(userId, "Интеграция добавлена");
            context.CurrentState = null;
        }
        catch (Exception)
        {
            await BotStillNotInTheChannelAnswer(context, userId);
            return;
        }
    }

    private async Task BotStillNotInTheChannelAnswer(BotContext context, long userId)
    {
        await context.Bot.SendMessage(userId, "Бот все ещё не добавлен в канал. Попробуйте ещё раз или напишите stop для отмены");
    }
}

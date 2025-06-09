using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Context;
using TelegramBot.Context.States;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Handlers;

internal class EnterChatHandler : IHandler<Message>
{
    [State(typeof(UserAddIntegrationChatState), UserAddIntegrationChatState.ChatInput)]
    public async Task HandleAsync(BotContext context, Message update, CancellationToken cancellationToken)
    {
        long userId = update.From!.Id;
        if (update.Text == "stop")
        {
            await HandleStop(context, userId);
            return;
        }
        
        if (update.ForwardFromChat is null 
            || update.ForwardFromChat.Type != ChatType.Channel)
        { // not forwarded from chat
            await context.Bot.SendMessage(userId, "Неправильный формат сообщения. Попробуйте снова! (Или напишите stop для отмены)");
            return;
        }

        // forwarder from chat
        long chatId = update.ForwardFromChat.Id;
        var membership = await GetChatMemberInfoAsync(context, chatId);
        
        if (membership is not null)
        {
            await context.Bot.SendMessage(userId, "Интеграция добавлена!");
            return;
        }

        context.CurrentState = UserAddIntegrationChatState.RetryAddBotToChat;
        context.Storage["chatId"] = chatId;
        await context.Bot.SendMessage(userId, "Бот ещё не добавлен в данный канал. Добавьте бота в канал и отправьте мне любое сообщение! (Для отмены напишите <b>stop</b>)",
            parseMode: ParseMode.Html);
    }

    private async Task<ChatMember?> GetChatMemberInfoAsync(BotContext context, long chatId)
    {
        var me = await context.Bot.GetMe();

        try
        {
            var membership = await context.Bot.GetChatMember(chatId, me.Id);

            return membership;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task HandleStop(BotContext context, long userId)
    {
        context.CurrentState = null;
        await context.Bot.SendMessage(userId, "Операция отменена");
    }
}

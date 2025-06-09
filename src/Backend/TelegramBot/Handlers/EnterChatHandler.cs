using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api;
using TelegramBot.Context;
using TelegramBot.Context.States;
using TelegramBot.Persistence;
using TelegramBot.Persistence.Entites;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Handlers;

internal class EnterChatHandler : IHandler<Message>
{
    private readonly UsersService _usersService;
    private readonly ApiService _apiService;

    public EnterChatHandler(UsersService usersService, ApiService apiService)
    {
        _usersService = usersService;
        _apiService = apiService;
    }

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
        
        if (membership is not null && membership.IsAdmin)
        {
            Chat chat = await context.Bot.GetChat(chatId);
            string chatName = chat.Title!;

            bool result = await AddChat(context, chatId, userId, chatName);
            string answer = result
                ? "Интеграция добавлена"
                : "Что-то пошло не так!";
            await context.Bot.SendMessage(userId, answer);
            context.CurrentState = null;
            return;
        }

        context.CurrentState = UserAddIntegrationChatState.RetryAddBotToChat;
        context.Storage["chatId"] = chatId;
        await context.Bot.SendMessage(userId, "Бот ещё не добавлен в данный канал или не является администратором. Добавьте бота в канал и отправьте мне любое сообщение! (Для отмены напишите <b>stop</b>)",
            parseMode: ParseMode.Html);
    }

    private async Task<bool> AddChat(BotContext context, long chatId, long userId, string channelName)
    {
        PersistenceUser? user = await _usersService.GetUserByTelegramId(userId);
        if (user is null)
            return false;

        return await _apiService.AttachTelegramChannel(user, chatId.ToString(), channelName);
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

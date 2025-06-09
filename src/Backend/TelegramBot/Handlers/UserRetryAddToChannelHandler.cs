using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Api;
using TelegramBot.Context;
using TelegramBot.Context.States;
using TelegramBot.Persistence.Entites;
using TelegramBot.Persistence;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Handlers;

internal class UserRetryAddToChannelHandler : IHandler<Message>
{
    private readonly UsersService _usersService;
    private readonly ApiService _apiService;

    public UserRetryAddToChannelHandler(UsersService usersService, ApiService apiService)
    {
        _usersService = usersService;
        _apiService = apiService;
    }

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
            Chat chat = await context.Bot.GetChat(chatId);
            string chatName = chat.Title!;
            bool result = await AddChat(context, chatId, userId, chatName);
            string answer = result
                ? "Интеграция добавлена"
                : "Что-то пошло не так!";
            await context.Bot.SendMessage(userId, answer);
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

    private async Task<bool> AddChat(BotContext context, long chatId, long userId, string channelName)
    {
        PersistenceUser? user = await _usersService.GetUserByTelegramId(userId);
        if (user is null)
            return false;

        return await _apiService.AttachTelegramChannel(user, chatId.ToString(), channelName);
    }
}

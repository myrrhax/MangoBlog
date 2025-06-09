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
    [State(typeof(UserChatInputState))]
    public async Task HandleAsync(BotContext context, Message update, CancellationToken cancellationToken)
    {
        context.Storage["userId"] = update.From!.Id.ToString();
        if (update.ForwardFromChat?.Type == ChatType.Channel)
        {
            long chatId = update.ForwardFromChat.Id;
            context.Storage["chatId"] = chatId.ToString();
            context.Storage["chatName"] = update.ForwardFromChat.Title!;

            var bot = await context.Bot.GetMe();
            bool chatMembership = false;
            try
            {
                chatMembership = await context.Bot.GetChatMember(chatId, bot.Id) is not null;
            }
            catch (Exception ex)
            {
                chatMembership = false;
            }
            context.Storage["isMember"] = chatMembership;
            
            if (chatMembership)
            {
                await context.Bot.SendMessage(update.From.Id, "Интеграция добавлена");
            }
            else
            {
                await context.Bot.SendMessage(update.From.Id, "Бот ещё не добавлен в канал. Добавьте бота в чат и пришлите любое сообщение (или напишите <b>stop</b> для остановки)",
                    parseMode: ParseMode.Html);
            }
        }
        else
        {
            await context.Bot.SendMessage(update.From.Id, "Не удается добавить канал, воспользуйтеся /start заново, после чего перешлите сообщение с канала");
        }

        context.CurrentState!.Handle();
    }
}

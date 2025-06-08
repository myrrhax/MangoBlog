﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api;
using TelegramBot.Context;
using TelegramBot.Persistence;
using TelegramBot.Routing;
using TelegramBot.Routing.Attributes;

namespace TelegramBot.Handlers;

internal class CommandStartHandler : IHandler<Message>
{
    private readonly ApiService _apiService;
    private readonly UsersService _usersService;

    public CommandStartHandler(ApiService apiService,
        UsersService usersService)
    {
        _apiService = apiService;
        _usersService = usersService;
    }

    [Command("/start")]
    public async Task HandleAsync(BotContext context, Message update, CancellationToken cancellationToken)
    {
        if (update.Text is null)
            return;
        string[] messageSplit = update.Text.Split(' ');
        if (messageSplit.Length == 1)
            await AnswerInformation(context, update, cancellationToken);
        else if (messageSplit.Length == 2)
            await AnswerWithConfirmation(context, update, messageSplit[1], cancellationToken);
    }

    private async Task AnswerInformation(BotContext context, Message msg, CancellationToken cancellationToken)
    {
        string answer = $"Здравствуйте, {msg.From!.FirstName}! Данный бот предназначен для интеграции с <b>MangoBlog</b>.\n" +
            $"Вы ещё не авторизованы в системе. Для работы пропишите /start ВАШ_КОД_ДЛЯ_ИНТЕГРАЦИИ, или перейдите по ссылке во вкладке <i>Интеграции</i> в приложении.";
        Stream logo = LoadLogo();
        
        await context.Bot.SendPhoto(msg.Chat.Id, photo: logo, caption: answer, parseMode: ParseMode.Html);
    }

    private async Task AnswerWithConfirmation(BotContext context, Message msg, string token, CancellationToken cancellationToken)
    {

    }

    private Stream LoadLogo()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "static/Logo.png");
        FileStream stream = File.OpenRead(path);

        return stream;
    }
}

using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Utils;

internal static class Keyboards
{
    public const string AddChannelQuery = "add-channel";

    public static readonly InlineKeyboardMarkup UserInegrationKeyboard = new(
        [
            [new InlineKeyboardButton("Добавить канал", AddChannelQuery), new InlineKeyboardButton("Информации об интеграции", "integration-info")],
            [new InlineKeyboardButton("Мои каналы", "my-channels")]
        ]
    );
}

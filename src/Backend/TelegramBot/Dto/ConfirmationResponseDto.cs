namespace TelegramBot.Dto;

internal record ConfirmationResponseDto(string BotToken, 
    UserDto User, 
    TelegramChannelDto Telegram);

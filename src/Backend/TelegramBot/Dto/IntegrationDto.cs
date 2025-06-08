namespace TelegramBot.Dto;

public record IntegrationDto(UserDto User, TelegramIntegrationDto Telegram);

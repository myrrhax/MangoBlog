namespace Application.Dto.Integrations;

public record IntegrationDto(UserDto user, TelegramIntegrationDto? Telegram);

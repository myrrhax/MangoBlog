namespace TelegramBot.Dto;

public record TelegramIntegrationDto(string IntegrationCode,
    string? TelegramId,
    bool IsConnected,
    IEnumerable<TelegramChannelDto> Channels);

namespace TelegramBot.Dto;

internal record TelegramIntegrationDto(string IntegrationCode,
    string? TelegramId,
    bool IsConnected,
    IEnumerable<TelegramChannelDto> Channels);

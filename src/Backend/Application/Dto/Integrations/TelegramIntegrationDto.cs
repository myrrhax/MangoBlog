namespace Application.Dto.Integrations;

public record TelegramIntegrationDto(string IntegrationCode, 
    string? TelegramId, 
    bool IsConnected,
    IEnumerable<TelegramChannelDto> Channels);

using Application.Dto.Integrations;

namespace Application.Dto;

public record ConfirmIntegrationResponse(string BotToken, IntegrationDto Integration);

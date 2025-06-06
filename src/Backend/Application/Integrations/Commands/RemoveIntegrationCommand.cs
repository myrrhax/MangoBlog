namespace Application.Integrations.Commands;

public record RemoveIntegrationCommand(Guid CallerId, string IntegrationType, string RoomId);
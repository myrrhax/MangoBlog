namespace WebApi.Dto;

public record ConfirmPublicationRequestDto(string PublicationId,
    string RoomId,
    string IntegrationType,
    string MessageId);

namespace Application.Dto;

public record PublicationDto(string Id,
    Guid UserId,
    string Content,
    IEnumerable<Guid> MediaIds,
    DateTime CreationDate,
    DateTime? PublicationTime,
    IEnumerable<IntegrationPublishInfoDto> Integrations);

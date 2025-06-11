using Domain.Enums;

namespace Application.Dto;

public record PublicationDto(string Id,
    Guid UserId,
    string Content,
    IEnumerable<(Guid, MediaFileType)> MediaIds,
    DateTime CreationDate,
    DateTime? PublicationTime,
    IEnumerable<IntegrationPublishInfoDto> Integrations);

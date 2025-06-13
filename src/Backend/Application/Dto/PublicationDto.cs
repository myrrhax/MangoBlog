using Domain.Enums;

namespace Application.Dto;

public record PublicationDto(string Id,
    Guid UserId,
    string Content,
    IEnumerable<MediaFileDto> Medias,
    DateTime CreationDate,
    DateTime? PublicationTime,
    IEnumerable<IntegrationPublishInfoDto> Integrations);

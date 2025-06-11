using Domain.Entities;

namespace Application.Dto;

public record IntegrationPublishInfoDto(string IntegrationType,
    IEnumerable<RoomPublishStatus> RoomStatuses);

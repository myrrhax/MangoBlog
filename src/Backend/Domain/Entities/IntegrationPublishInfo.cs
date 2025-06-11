using Domain.Enums;

namespace Domain.Entities;

public class IntegrationPublishInfo
{
    public IntegrationType IntegrationType { get; set; }
    public List<RoomPublishStatus> PublishStatuses { get; set; } = [];
}

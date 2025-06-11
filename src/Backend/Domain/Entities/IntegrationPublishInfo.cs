using Domain.Enums;

namespace Domain.Entities;

public class IntegrationPublishInfo
{
    public IntegrationType IntegrationType { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<RoomPublishStatus> PublishStatuses { get; set; } = [];
}

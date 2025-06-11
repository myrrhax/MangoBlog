using Domain.Enums;

namespace Domain.Entities;

public class IntegrationPublishInfo
{
    public IntegrationType IntegrationType { get; set; }
    public Dictionary<string, string> RoomMessagesIds { get; set; } = [];
}

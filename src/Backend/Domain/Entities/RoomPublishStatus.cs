namespace Domain.Entities;

public class RoomPublishStatus
{
    public string RoomId { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
}

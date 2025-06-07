namespace Domain.Entities;

public class TelegramChannel
{
    public int Id { get; set; }
    public string RoomId { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public int TelegramIntegrationId { get; set; }

    public TelegramIntegration Integration { get; set; } = null!;
}

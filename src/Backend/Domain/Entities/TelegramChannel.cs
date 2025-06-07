namespace Domain.Entities;

public class TelegramChannel
{
    public int Id { get; set; }
    public string ChannelId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int TelegramIntegrationId { get; set; }

    public TelegramIntegration TelegramIntegration { get; set; } = null!;
}

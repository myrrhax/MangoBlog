namespace Domain.Entities;

public class TelegramIntegration
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int IntegrationId { get; set; }
    public string TelegramId { get; set; } = string.Empty;
    public ICollection<TelegramChannel> ConnectedChannels { get; set; } = [];
    public string IntegrationCode { get; set; } = string.Empty;
    public bool IsConnected { get; set; }

    public Integration Integration { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}

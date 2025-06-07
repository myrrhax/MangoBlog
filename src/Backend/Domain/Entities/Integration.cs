namespace Domain.Entities;

public class Integration
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int? TelegramIntegrationId { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public TelegramIntegration? TelegramIntegration { get; set; }
}

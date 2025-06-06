namespace Domain.Entities;

public class UserIntegration
{
    public int IntegrationId { get; set; }
    public Guid UserId { get; set; }
    public string? ApiToken { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public string ConfirmationCode { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }

    public Integration Integration { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public ICollection<IntegrationConnectedRooms> ConnectedRooms { get; set; } = [];
}

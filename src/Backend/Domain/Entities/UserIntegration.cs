namespace Domain.Entities;

public class UserIntegration
{
    public int IntegrationId { get; set; }
    public Guid UserId { get; set; }
    public string? ApiToken { get; set; }
    public string? AccountId { get; set; }
    public string? ConfirmationCode { get; set; }
    public bool IsConfirmed { get; set; }
    public string RoomId { get; set; } = string.Empty;

    public Integration Integration { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}

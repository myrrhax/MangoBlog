namespace Domain.Entities;

public class UserIntegration
{
    public int Id { get; set; }
    public int IntegrationId { get; set; }
    public Guid UserId { get; set; }
    public string? ApiToken { get; set; }
    public string? AccountId { get; set; }
    public string? ConfirmationCode { get; set; }
    public bool IsConfirmed { get; set; }
    public string RoomId { get; set; } = string.Empty;

    public Integration Integration { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public UserIntegration(Integration integration, 
        ApplicationUser user, 
        string? apiToken = null,
        string? accountId = null,
        string? confirmationCode = null,
        bool isConfirmed = false,
        string roomId = "")
    {
        IntegrationId = integration.Id;
        Integration = integration;
        User = user;
        UserId = user.Id;
        ApiToken = apiToken;
        AccountId = accountId;
        ConfirmationCode = confirmationCode;
        IsConfirmed = isConfirmed;
        RoomId = roomId;
    }

    public UserIntegration()
    {
        
    }
}

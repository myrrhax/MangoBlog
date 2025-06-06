namespace Application.Dto;

public class IntegrationDto
{
    public string IntegrationType { get; set; } = string.Empty;
    public string? AccountId { get; set; }
    public bool IsConfirmed { get; set; }
    public string RoomId { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
}

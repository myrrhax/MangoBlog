namespace Application.Dto;

public class IntegrationDto
{
    public string IntegrationType { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
    public IEnumerable<string> ConnectedRooms { get; set; } = [];
}

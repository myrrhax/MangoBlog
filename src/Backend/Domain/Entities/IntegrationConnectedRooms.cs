namespace Domain.Entities;

public class IntegrationConnectedRooms
{
    public UserIntegration UserIntegration { get; set; } = null!;
    public string RoomId { get; set; } = string.Empty;
}

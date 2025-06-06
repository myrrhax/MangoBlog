namespace Domain.Entities;

public class IntegrationConnectedRooms
{
    public int Id { get; set; }
    public UserIntegration UserIntegration { get; set; } = null!;
    public string RoomId { get; set; } = string.Empty;
}

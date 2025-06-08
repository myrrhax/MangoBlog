namespace TelegramBot.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public string DisplayedName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? AvatarId { get; set; }
    public DateOnly? BirthDate { get; set; }
}

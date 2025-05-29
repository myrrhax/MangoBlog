using Domain.Entities;

namespace Application.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public string DisplayedName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateOnly? BirthDate { get; set; }
}

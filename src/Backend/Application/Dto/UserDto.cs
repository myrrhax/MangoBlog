namespace Application.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public required string DisplayedName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string AvatarUrl { get; set; }
    public DateOnly? BirthDate { get; set; }
}

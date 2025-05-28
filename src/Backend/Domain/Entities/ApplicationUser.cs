namespace Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }
    public required string DisplayedName { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public DateTime RegistrationTime { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}

namespace Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }
    public string DisplayedName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public DateTime RegistrationTime { get; set; }
}

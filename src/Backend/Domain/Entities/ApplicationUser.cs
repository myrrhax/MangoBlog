using Domain.Enums;

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
    public Guid? AvatarId { get; set; }
    public MediaFile? Avatar { get; set; }
    public DateOnly? BirthDate { get; set; }
    public DateTime RegistrationTime { get; set; }
    public Role Role { get; set; }
    public ICollection<MediaFile> MediaFiles { get; set; } = [];
    public ICollection<ApplicationUser> Subscriptions { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Integration> Integrations { get; set; } = [];

    public ApplicationUser(string login, string email, string hash, string firstName, string lastName, 
        MediaFile? avatar = default, DateOnly? birthDate = default, Role role = Role.User)
    {
        Id = Guid.NewGuid();
        Email = email;
        Login = login;
        DisplayedName = login;
        PasswordHash = hash;
        FirstName = firstName;
        LastName = lastName;
        Avatar = avatar;
        AvatarId = avatar?.Id;
        BirthDate = birthDate;
        RegistrationTime = DateTime.UtcNow;
        Role = role;
    }

    public ApplicationUser()
    {
        
    }
}

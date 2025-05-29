namespace Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }

    public ApplicationUser User { get; set; } = null!;

    internal RefreshToken()
    {
        
    }

    public RefreshToken(string token, ApplicationUser user, DateTime expirationDate)
    {
        Id = Guid.NewGuid();
        UserId = user.Id;
        Token = token;
        ExpirationDate = expirationDate;
        User = user;
    }
}

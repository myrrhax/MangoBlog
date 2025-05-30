namespace Infrastructure.Utils;

public class JwtConfig
{
    public string Issuer { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationTimeMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
    public int RefreshTokenMaxSessionsCount { get; set; }
    public string CookieName { get; set; } = string.Empty;
}

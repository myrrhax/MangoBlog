namespace Infrastructure.Utils;

public class JwtConfig
{
    public string Issuer { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationTimeMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
    public int RefreshTokenMaxSessionsCount { get; set; }
}

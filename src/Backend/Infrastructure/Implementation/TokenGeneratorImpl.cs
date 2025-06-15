using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Implementation;

internal class TokenGeneratorImpl(IOptions<JwtConfig> jwtConfig) : ITokenGenerator
{
    public string GenerateAccessToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString().ToLower()),
        };
        DateTime expirationDate = DateTime.UtcNow.AddSeconds(30);
        
        return WriteJwtToken(claims, expirationDate);
    }

    public RefreshToken GenerateRefreshToken(ApplicationUser user)
    {
        (string token, DateTime expiration) = GenerateRefreshToken();
        
        return new RefreshToken(token, user, expiration);
    }

    public (string, DateTime) GenerateRefreshToken()
    {
        return (GetBaseString(), DateTime.UtcNow.AddDays(jwtConfig.Value.RefreshTokenExpirationDays));
    }

    private static string GetBaseString()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string GenerateIntegrationBotToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(jwtConfig.Value.BotTokenClaimName, "true"),
        };
        DateTime expiration = DateTime.UtcNow.AddDays(100); // Бессрочный токен

        return WriteJwtToken(claims, expiration);
    }

    private string WriteJwtToken(List<Claim> claims, DateTime expirationDate)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Value.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtConfig.Value.Issuer,
            audience: jwtConfig.Value.Audience,
            claims: claims,
            expires: expirationDate,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

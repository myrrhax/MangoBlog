using Domain.Entities;

namespace Application.Abstractions;

public interface ITokenGenerator
{
    RefreshToken GenerateRefreshToken(ApplicationUser user);
    (string, DateTime) GenerateRefreshToken();
    string GenerateAccessToken(ApplicationUser user);
    string GenerateIntegrationBotToken(ApplicationUser user);
}

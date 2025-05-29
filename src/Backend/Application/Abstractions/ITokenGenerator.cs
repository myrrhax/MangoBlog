using Domain.Entities;

namespace Application.Abstractions;

public interface ITokenGenerator
{
    string GenerateRefreshToken();
    string GenerateAccessToken(ApplicationUser user);
}

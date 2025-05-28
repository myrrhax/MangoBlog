using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;

public interface IUserRepository
{
    Task<Result<ApplicationUser>> GetUserInfoById(Guid id, CancellationToken cancellationToken);
    Task<Result<ApplicationUser>> GetUserById(Guid id, CancellationToken cancellationToken);
    Task<Result<ApplicationUser>> GetUserByLogin(string login, CancellationToken cancellationToken);
    Task<Result> AddUser(ApplicationUser user, CancellationToken cancellationToken);
    Task<Result> UpdateUser(ApplicationUser user, CancellationToken cancellationToken);
    Task<Result> DeleteUserById(Guid userId, CancellationToken cancellationToken);
    Task<Result> AddRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<Result<RefreshToken>> GetRefreshToken(string token, Guid userId, CancellationToken cancellationToken);
    Task<Result> UpdateRefreshToken(RefreshToken token, CancellationToken cancellationToken);
    Task<Result> DeleteRefreshToken(string token, Guid userId, CancellationToken cancellationToken);
}
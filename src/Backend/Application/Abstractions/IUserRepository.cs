using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;

public interface IUserRepository
{
    Task<ApplicationUser?> GetUserById(Guid id, CancellationToken cancellationToken);
    Task<ApplicationUser?> GetUserByLogin(string login, CancellationToken cancellationToken);
    Task<Result> AddUser(ApplicationUser user, CancellationToken cancellationToken);
    Task<Result> DeleteUserById(Guid userId, CancellationToken cancellationToken);
    Task<bool> IsEmailTaken(string email, CancellationToken cancellationToken);
    Task<bool> IsLoginTaken(string login, CancellationToken cancellationToken);
    Task<Result> AddRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetRefreshToken(string token, CancellationToken cancellationToken);
    Task<Result> UpdateRefreshToken(Guid tokenId, string newToken, DateTime newExpirationTime, CancellationToken cancellationToken);
    Task<Result> DeleteRefreshToken(RefreshToken token, CancellationToken cancellationToken);
    Task<Dictionary<string, ApplicationUser?>> LoadAuthors(Dictionary<string, Guid> articleAuthors, CancellationToken cancellationToken);
    Task<Result> AddSubscription(Guid subscriberId, Guid userId, CancellationToken cancellationToken);
    Task<Result> RemoveSubscription(Guid subscriberId, Guid userId, CancellationToken cancellationToken);
    Task<Result> ChangeAvatar(Guid userId, Guid avatarId, CancellationToken cancellationToken);
}
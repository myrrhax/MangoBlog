using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Infrastructure.DataContext;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Implementation;

internal class UserRepositoryImpl(ApplicationDbContext context, ILogger<UserRepositoryImpl> logger) : IUserRepository
{
    public async Task<Result> AddRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
        }
    }

    public Task<Result> AddUser(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteRefreshToken(RefreshToken token, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteUserById(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> GetRefreshToken(string token, Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApplicationUser?> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApplicationUser?> GetUserByLogin(string login, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsEmailTaken(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsLoginTaken(string login, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateRefreshToken(RefreshToken token, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateUser(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

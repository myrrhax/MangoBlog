using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
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
            logger.LogInformation("New refresh token was added to user with id: {}", refreshToken.UserId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to add new refresh token to user with id: {}.\nError message: {}\n.Stack trace: {}",
                refreshToken.UserId, ex.Message, ex.StackTrace);
            return Result.Failure(new InsertionError(string.Empty));
        }
    }

    public async Task<Result> AddUser(ApplicationUser user, CancellationToken cancellationToken)
    {
        try
        {
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("New user was added to database with id: {}", user.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to add new user to database.\nError message: {}\n. Stack trace: {}",
                ex.Message, ex.StackTrace);
            return Result.Failure(new InsertionError(string.Empty));
        }
    }

    public async Task<Result> DeleteRefreshToken(RefreshToken token, CancellationToken cancellationToken)
    {
        int rows = await context.RefreshTokens
            .Where(entity => entity.Id == token.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return rows > 0
            ? Result.Success()
            : Result.Failure();
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

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
            return Result.Failure(new DatabaseInteractionError("Unable to insert new refresh token"));
        }
    }

    public async Task<Result> AddSubscription(Guid subscriberId, Guid userId, CancellationToken cancellationToken)
    {
        ApplicationUser subscriber = await context.Users
            .Include(user => user.Subscriptions)
            .FirstAsync(user => user.Id == subscriberId);
        
        ApplicationUser? userChannel = await context.Users.FindAsync(userId);
        if (userChannel is null)
            return Result.Failure(new UserNotFound());

        try
        {
            if (subscriber.Subscriptions.Contains(userChannel))
            {
                return Result.Failure(new SubscriptionAlreadyExists(subscriberId, userId));
            }
            subscriber.Subscriptions.Add(userChannel);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to add new subscription (SubscriberId: {}, ChannelId: {}).\nError: {}", 
                subscriberId, 
                userId,
                ex.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to add subscription"));
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
            return Result.Failure(new DatabaseInteractionError("Unable to insert new user"));
        }
    }

    public async Task<Result> ChangeAvatar(Guid userId, Guid avatarId, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.Users
                .Where(user => user.Id == userId)
                .ExecuteUpdateAsync(user => user.SetProperty(prop => prop.AvatarId, avatarId));

            return rows > 0
                ? Result.Success()
                : Result.Failure(new UserNotFound());
        }
        catch (Exception e)
        {
            logger.LogError("Failed to change avatar for user: {}. Error: {}", userId, e.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to change avatar"));
        }
    }

    public async Task<Result<int>> DeleteExpiredTokens(CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.RefreshTokens
                .Where(token => token.ExpirationDate <= DateTime.UtcNow)
                .ExecuteDeleteAsync(cancellationToken);

            return Result.Success(rows);
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new DatabaseInteractionError($"Failed to delete expired tokens {ex.Message}"));
        }
    }

    public async Task<Result> DeleteRefreshToken(RefreshToken token, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.RefreshTokens
                .Where(entity => entity.Id == token.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (rows > 0)
            {
                logger.LogInformation("Refresh token: {} was successfully deleted from user with id: {}", 
                    token.Token, token.UserId);
                return Result.Success();
            }
            logger.LogInformation("Refresh token was not found");
            return Result.Failure(new InvalidToken());
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to delete refresh token: {} from user with id: {}.\nError message: {}\nStack trace: {}",
                token.Token, token.UserId, ex.Message, ex.StackTrace);

            return Result.Failure(new DatabaseInteractionError("Unable to delete token"));
        }
    }

    public async Task<Result> DeleteUserById(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.Users
                .Where(entity => entity.Id == userId)
                .ExecuteDeleteAsync(cancellationToken);

            if (rows > 0)
            {
                logger.LogInformation("User with id: {} was successfully deleted.",
                    userId);
                return Result.Success();
            }
            logger.LogInformation("User was not found");
            return Result.Failure(new UserNotFound());
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to delete user with id: {}.\nError message: {}\nStack trace: {}",
                userId, ex.Message, ex.StackTrace);

            return Result.Failure(new DatabaseInteractionError("Unable to delete user"));
        }
    }

    public async Task<RefreshToken?> GetRefreshToken(string token, CancellationToken cancellationToken)
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .Include(token => token.User)
            .FirstOrDefaultAsync(entity => entity.Token == token, cancellationToken);
    }

    public async Task<ApplicationUser?> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        return await context.Users
            .Include(user => user.RefreshTokens)
            .Include(user => user.Subscriptions)
            .Include(user => user.Avatar)
            .Include(user => user.Integration)
            .ThenInclude(integration => integration!.TelegramIntegration)
            .ThenInclude(tgIntegration => tgIntegration!.ConnectedChannels)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<ApplicationUser?> GetUserByLogin(string login, CancellationToken cancellationToken)
    {
        return await context.Users
            .Include(user => user.Subscriptions)
            .Include(user => user.Avatar)
            .FirstOrDefaultAsync(user => user.Login == login, cancellationToken);
    }

    public async Task<bool> IsEmailTaken(string email, CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<bool> IsLoginTaken(string login, CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Login == login, cancellationToken);
    }

    public async Task<Dictionary<string, ApplicationUser?>> LoadAuthors(Dictionary<string, Guid> authorIds, CancellationToken cancellationToken)
    {
        List<ApplicationUser> users = await context.Users
            .Where(user => authorIds.Values.Contains(user.Id))
            .ToListAsync();

        return authorIds.Keys
            .ToDictionary(key => key,
                key => users.FirstOrDefault(user => user.Id == authorIds[key]));
    }

    public async Task<Result> RemoveSubscription(Guid subscriberId, Guid userId, CancellationToken cancellationToken)
    {
        ApplicationUser subscriber = await context.Users
           .Include(user => user.Subscriptions)
           .FirstAsync(user => user.Id == subscriberId);

        ApplicationUser? userChannel = await context.Users.FindAsync(userId);
        if (userChannel is null)
            return Result.Failure(new UserNotFound());

        try
        {
            if (!subscriber.Subscriptions.Contains(userChannel))
            {
                return Result.Failure(new SubscriptionNotFound(subscriberId, userId));
            }
            subscriber.Subscriptions.Remove(userChannel);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to remove subscription (SubscriberId: {}, ChannelId: {}).\nError: {}",
                subscriberId,
                userId,
                ex.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to remove subscription"));
        }
    }

    public async Task<Result> UpdateRefreshToken(Guid tokenId, string newToken, DateTime newExpirationDate, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.RefreshTokens
                .Where(token => token.Id == tokenId)
                .ExecuteUpdateAsync(entity => entity
                    .SetProperty(property => property.Token, newToken)
                    .SetProperty(property => property.ExpirationDate, newExpirationDate)
                );

            if (rows > 0)
            {
                logger.LogInformation("Token with id: {} was successfully deleted.",
                    tokenId);
                return Result.Success();
            }
            logger.LogInformation("Token was not found");
            return Result.Failure(new InvalidToken());
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to update refresh token with id: {}.\nError message: {}\n. Stack trace: {}",
                tokenId, ex.Message, ex.StackTrace);
            return Result.Failure(new DatabaseInteractionError("Unable to update refresh token"));
        }
        
    }
}

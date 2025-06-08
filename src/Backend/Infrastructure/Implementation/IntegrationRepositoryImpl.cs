using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Implementation;

internal class IntegrationRepositoryImpl(ApplicationDbContext context,
    ILogger<IntegrationRepositoryImpl> logger) : IIntegrationRepository
{
    public async Task<Result> AddTelegramIntegration(TelegramIntegration integration, CancellationToken cancellationToken)
    {
        try
        {
            await context.TelegramIntegration.AddAsync(integration);
            await context.SaveChangesAsync();

            logger.LogInformation("Integration successfully added for user: {}", integration.UserId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to add tg integration for user: {}. Error: {}",
                integration.UserId,
                ex.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to add new tg integration"));
        }
    }

    public async Task<Result<Integration>> ConfirmTelegramIntegration(string integrationCode, string telegramId, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.TelegramIntegration
                .Where(integration => !integration.IsConfirmed
                    && integration.IntegrationCode == integrationCode)
                .ExecuteUpdateAsync(userIntegration => userIntegration
                    .SetProperty(prop => prop!.IsConfirmed, true)
                    .SetProperty(prop => prop!.TelegramId, telegramId));

            if (rows == 0)
                return Result.Failure<Integration>(new IntegrationNotFound());

            Integration entity = await context.Integrations
                .Include(integration => integration.User)
                .ThenInclude(user => user.Avatar)
                .Include(integration => integration.TelegramIntegration)
                .FirstAsync(integration => integration.TelegramIntegration!.IntegrationCode == integrationCode);

            return Result.Success(entity);
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to confirm integration with code: {}. Error: {}", integrationCode, ex.Message);

            return Result.Failure<Integration>(new DatabaseInteractionError("Unable to confirm integration"));
        }
    }

    public Task<Result> DeleteFromTelegramChannel(Guid userId, string channelId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteTelegramIntegration(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.TelegramIntegration
                .Where(entity => entity.UserId == userId)
                .ExecuteDeleteAsync();

            return rows > 0
                ? Result.Success()
                : Result.Failure(new IntegrationNotFound());
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to delete telegram integration for user with id: {}. Error: {}", userId, ex.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to delete telegram integration"));
        }
    }

    public Task<Integration?> GetIntegrationByTelegramId(string telegramId, CancellationToken cancellationToken)
    {
        return context.Integrations
            .Include(integr => integr.TelegramIntegration)
            .Include(integr => integr.User)
            .FirstOrDefaultAsync(integr => integr.TelegramIntegration.TelegramId == telegramId);
    }

    public Task<TelegramIntegration?> GetTelegramIntegration(Guid userId, CancellationToken cancellationToken)
        => context.TelegramIntegration
            .FirstOrDefaultAsync(integration => integration.UserId == userId);
}

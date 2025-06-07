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

    public async Task<Result> ConfirmTelegramIntegration(string integrationCode, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.Integrations
                .Include(integration => integration.TelegramIntegration)
                .Where(integration => integration.TelegramIntegration != null)
                .Where(integration => !integration.TelegramIntegration!.IsConfirmed
                    && integration.TelegramIntegration.IntegrationCode == integrationCode)
                .ExecuteUpdateAsync(userIntegration => userIntegration.SetProperty(prop => prop.TelegramIntegration!.IsConfirmed, true));

            return rows > 0
                ? Result.Success()
                : Result.Failure(new IntegrationNotFound());
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to confirm integration with code: {}. Error: {}", integrationCode, ex.Message);

            return Result.Failure(new DatabaseInteractionError("Unable to confirm integration"));
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

    public Task<TelegramIntegration?> GetTelegramIntegration(Guid userId, CancellationToken cancellationToken)
        => context.TelegramIntegration
            .FirstOrDefaultAsync(integration => integration.UserId == userId);
}

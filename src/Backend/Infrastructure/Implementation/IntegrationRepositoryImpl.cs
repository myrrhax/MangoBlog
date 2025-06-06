using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Implementation;

internal class IntegrationRepositoryImpl(ApplicationDbContext context,
    ILogger<IntegrationRepositoryImpl> logger) : IIntegrationRepository
{
    public async Task<Result> AddIntegration(UserIntegration integration, CancellationToken cancellationToken)
    {
        try
        {
            await context.UsersIntegrations.AddAsync(integration, cancellationToken);
            await context.SaveChangesAsync();

            logger.LogInformation("Successfully created new integration {} for user: {}",
                integration.Integration.IntegrationType.ToString(),
                integration.UserId.ToString());
            return Result.Success();
        }
        catch(Exception ex)
        {
            logger.LogError("An error occurred creating new integration for user: {}. Error: {}",
                integration.UserId,
                ex.Message);

            return Result.Failure(new DatabaseInteractionError("Unable to add new integration"));
        }
    }

    public async Task<Result> ConfirmIntegration(string integrationCode, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.UsersIntegrations
                .Where(userIntegration => !userIntegration.IsConfirmed
                    && userIntegration.ConfirmationCode == integrationCode)
                .ExecuteUpdateAsync(userIntegration => userIntegration.SetProperty(prop => prop.IsConfirmed, true));

            return rows > 0
                ? Result.Success()
                : Result.Failure(new IntegrationNotFound(integrationCode));
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to confirm integration with code: {}. Error: {}", integrationCode, ex.Message);

            return Result.Failure(new DatabaseInteractionError("Unable to confirm integration"));
        }
    }

    public async Task<Integration> GetIntegration(IntegrationType type, CancellationToken cancellationToken)
    {
        return await context.Integrations
            .FirstAsync(integration => integration.IntegrationType == type);
    }

    public async Task<UserIntegration?> GetIntegrationGroupId(IntegrationType type, string groupId, CancellationToken cancellationToken)
    {
        return await context.UsersIntegrations
            .Include(userIntegration => userIntegration.Integration)
            .FirstOrDefaultAsync(userIntegration => userIntegration.Integration.IntegrationType == type 
                && userIntegration.RoomId == groupId);
    }
}

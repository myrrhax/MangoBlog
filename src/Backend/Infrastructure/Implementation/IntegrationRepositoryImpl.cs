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
    public Task<Result> AddTelegramIntegration(TelegramIntegration integration, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> ConfirmTelegramIntegration(string integrationCode, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.Integrations
                .Include(integration => integration.TelegramIntegration)
                .Where(integration => integration.TelegramIntegration != null)
                .Where(integration => !integration.TelegramIntegration!.IsConnected
                    && integration.TelegramIntegration.IntegrationCode == integrationCode)
                .ExecuteUpdateAsync(userIntegration => userIntegration.SetProperty(prop => prop.TelegramIntegration!.IsConnected, true));

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

    public async Task<Result> DeleteIntegration(Guid userId, IntegrationType type, string roomId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Integration> GetIntegration(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

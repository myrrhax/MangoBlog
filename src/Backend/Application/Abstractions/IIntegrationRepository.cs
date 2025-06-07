using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IIntegrationRepository
{
    Task<Integration> GetIntegration(Guid userId, CancellationToken cancellationToken);
    Task<Result> AddTelegramIntegration(TelegramIntegration integration, CancellationToken cancellationToken);
    Task<Result> ConfirmTelegramIntegration(string integrationCode, CancellationToken cancellationToken);
    Task<Result> DeleteIntegration(Guid userId, IntegrationType type, string roomId, CancellationToken cancellationToken);
}

using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IIntegrationRepository
{
    Task<Integration> GetIntegration(IntegrationType type, CancellationToken cancellationToken);
    Task<Result> AddIntegration(UserIntegration integration, CancellationToken cancellationToken);
    Task<Result> ConfirmIntegration(string integrationCode, CancellationToken cancellationToken);
    Task<UserIntegration?> GetIntegrationGroupId(IntegrationType type, string groupId, CancellationToken cancellationToken);
}

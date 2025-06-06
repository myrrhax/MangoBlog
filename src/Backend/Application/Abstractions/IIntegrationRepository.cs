using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IIntegrationRepository
{
    Task<Result> AddIntegration(UserIntegration integration, CancellationToken cancellationToken);
    Task<Result> ConfirmIntegration(Guid UserId, IntegrationType type, CancellationToken cancellationToken);
}

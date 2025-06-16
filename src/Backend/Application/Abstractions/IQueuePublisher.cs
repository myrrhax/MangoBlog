using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IQueuePublisher
{
    Result Publish(Publication publication);
    Result PublishDeleteIntegration(IntegrationType type, Guid userId);
}

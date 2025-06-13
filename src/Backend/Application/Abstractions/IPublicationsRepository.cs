using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;
public interface IPublicationsRepository
{
    Task<Result> AddPublication(Publication publication);
    Task<Result> IsStatusUnconfirmed(Guid userId, string PublicationId, string ChannelId, IntegrationType type);
    Task<Result> ConfirmPublicationStatus(string PublicationId, string ChannelId, IntegrationType type);
}

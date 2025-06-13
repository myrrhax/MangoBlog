using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;
public interface IPublicationsRepository
{
    Task<Result> AddPublication(Publication publication);
    Task<Result> IsStatusUnconfirmed(Guid userId, string publicationId, string channelId, IntegrationType type);
    Task<Result> ConfirmPublicationStatus(string publicationId, string channelId, string messageId, IntegrationType type);
    Task<Publication?> GetPublicationById(string id);
}

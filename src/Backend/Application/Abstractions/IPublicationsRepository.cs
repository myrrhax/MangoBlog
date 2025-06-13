using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;
public interface IPublicationsRepository
{
    Task<Result> AddPublication(Publication publication);
    Task<Result<bool>> GetConfirmationStatus(Guid userId, string PublicationId, string ChannelId);
    Task<Result> ConfirmPublicationStatus(string PublicationId, string ChannelId);
}

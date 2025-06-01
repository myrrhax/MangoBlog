using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions;

public interface IRatingsRepository
{
    Task<int> GetPostLikesCount(string postId, CancellationToken cancellationToken);
    Task<int> GetPostDislikesCount(string postId, CancellationToken cancellationToken);
    Task<double> GetPostAverageRating(string postId, CancellationToken cancellationToken);
    Task<RatingType?> GetUserRating(Guid userId, string postId, CancellationToken cancellationToken);
    Task<IEnumerable<Rating>> GetUserRatings(Guid userId, CancellationToken cancellationToken);
}

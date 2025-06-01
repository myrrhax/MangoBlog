using Domain.Enums;

namespace Application.Abstractions;

public interface IRatingsRepository
{
    Task<int> GetPostLikesCount(Guid postId, CancellationToken cancellationToken);
    Task<int> GetPostDislikesCount(Guid postId, CancellationToken cancellationToken);
    Task<double> GetPostAverageRating(Guid postId, CancellationToken cancellationToken);
    Task<RatingType?> GetUserRating(Guid userId, CancellationToken cancellationToken);
}

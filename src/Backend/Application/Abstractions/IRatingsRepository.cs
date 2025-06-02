using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions;

public interface IRatingsRepository
{
    Task<(int, int)> GetPostLikesAndDislikes(string postId, CancellationToken cancellationToken);
    Task<double> GetPostAverageRating(string postId, CancellationToken cancellationToken);
    Task<RatingType?> GetUserRating(Guid userId, string postId, CancellationToken cancellationToken);
    Task<IEnumerable<Rating>> GetUserRatings(Guid userId, CancellationToken cancellationToken);
    Task<Dictionary<string, (int, int)>> GetRatingsForArticles(IEnumerable<Article> articles, CancellationToken cancellationToken);
}

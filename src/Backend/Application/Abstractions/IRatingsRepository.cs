using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IRatingsRepository
{
    Task<(int, int)> GetPostLikesAndDislikes(string postId, CancellationToken cancellationToken);
    Task<double> GetPostAverageRating(string postId, CancellationToken cancellationToken);
    Task<RatingType?> GetUserRating(Guid userId, string postId, CancellationToken cancellationToken);
    Task<IEnumerable<Rating>> GetUserRatings(Guid userId, CancellationToken cancellationToken);
    Task<Dictionary<string, (int, int)>> GetRatingsForArticles(IEnumerable<Article> articles, CancellationToken cancellationToken);
    Task<Result> DeletePostRatings(string postId, CancellationToken cancellationToken);
    Task<Result> UpdateRating(string postId, Guid userId, RatingType newRating, CancellationToken cancellationToken);
    Task<Result> AddRating(string postId, Guid userId, RatingType rating, CancellationToken cancellationToken);
    Task<Result> RemoveRatingFromPost(string postId, Guid userId, CancellationToken cancellationToken);
}

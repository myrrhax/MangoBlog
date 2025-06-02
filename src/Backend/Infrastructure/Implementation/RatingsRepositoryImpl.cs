using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementation;

internal class RatingsRepositoryImpl(ApplicationDbContext context) : IRatingsRepository
{
    public Task<double> GetPostAverageRating(string postId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<(int, int)> GetPostLikesAndDislikes(string postId, CancellationToken cancellationToken)
    {
        var groups = await context.Ratings
            .AsNoTracking()
            .Where(rating => rating.ArticleId == postId)
            .GroupBy(rating => rating.RatingType)
            .Select(group => new
            {
                Type = group.Key,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        int likes = groups.FirstOrDefault(g => g.Type == RatingType.Like)?.Count ?? 0;
        int dislikes = groups.FirstOrDefault(g => g.Type == RatingType.Dislike)?.Count ?? 0;

        return (likes, dislikes);
    }

    public async Task<Dictionary<string, (int, int)>> GetRatingsForArticles(
        IEnumerable<Article> articles,
        CancellationToken cancellationToken)
    {
        var articleIds = articles.Select(a => a.Id).ToList();

        var groupedRatings = await context.Ratings
            .AsNoTracking()
            .Where(r => articleIds.Contains(r.ArticleId))
            .GroupBy(r => new { r.ArticleId, r.RatingType })
            .Select(g => new
            {
                g.Key.ArticleId,
                g.Key.RatingType,
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        var result = articleIds.ToDictionary(id => id, id => (likes: 0, dislikes: 0));

        foreach (var rating in groupedRatings)
        {
            if (result.TryGetValue(rating.ArticleId, out var counts))
            {
                if (rating.RatingType == RatingType.Like)
                    counts.likes = rating.Count;
                else if (rating.RatingType == RatingType.Dislike)
                    counts.dislikes = rating.Count;

                result[rating.ArticleId] = counts;
            }
        }

        return result;
    }

    public async Task<RatingType?> GetUserRating(Guid userId, string postId, CancellationToken cancellationToken)
    {
        Rating? rating = await context.Ratings
            .AsNoTracking()
            .FirstOrDefaultAsync(rating => rating.UserId == userId && rating.ArticleId == postId);

        return rating is null
            ? null
            : rating.RatingType;
    }

    public Task<IEnumerable<Rating>> GetUserRatings(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

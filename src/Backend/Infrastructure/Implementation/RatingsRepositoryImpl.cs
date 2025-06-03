using Application.Abstractions;
using DnsClient.Internal;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Implementation;

internal class RatingsRepositoryImpl(ApplicationDbContext context, 
    ILogger<RatingsRepositoryImpl> logger) : IRatingsRepository
{
    public async Task<Result> AddRating(string postId, Guid userId, RatingType rating, CancellationToken cancellationToken)
    {
        var entity = new Rating()
        {
            ArticleId = postId,
            UserId = userId,
            RatingType = rating,
            CreationDate = DateTime.UtcNow,
        };

        try
        {
            await context.Ratings.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync();

            logger.LogInformation("New rating ({}, {} - {})", postId, userId, rating.ToString());
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to add new rating: ({}, {} - {}). Error: {}", postId, userId, rating.ToString(), ex.Message);

            return Result.Failure(new DatabaseInteractionError("Unable to add new rating"));
        }
    }

    public async Task<Result> DeletePostRatings(string postId, CancellationToken cancellationToken)
    {
        try
        {
            await context.Ratings
                .Where(rating => rating.ArticleId == postId)
                .ExecuteDeleteAsync();
            logger.LogInformation("Ratings for post: {} successfully deleted", postId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to delete post ratings (postId: {}). Error: {}", postId, ex.Message);

            return Result.Failure(new DatabaseInteractionError($"Unable to delete post: {postId}"));
        }
    }

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

    public async Task<IEnumerable<Rating>> GetUserRatings(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Ratings
            .Where(rating => rating.UserId == userId)
            .ToListAsync();
    }

    public async Task<Result> UpdateRating(string postId, Guid userId, RatingType newRating, CancellationToken cancellationToken)
    {
        try
        {
            int rows = await context.Ratings
                .Where(rating =>  rating.ArticleId == postId && rating.UserId == userId)
                .ExecuteUpdateAsync(rating => rating.SetProperty(rating => rating.RatingType, newRating));

            return rows > 0
                ? Result.Success()
                : Result.Failure(new RatingNotFound(postId));
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred updating rating ({}, {}). Error: {}", postId, userId, ex.Message);

            return Result.Failure(new DatabaseInteractionError("Failed to update rating"));
        }
    }
}

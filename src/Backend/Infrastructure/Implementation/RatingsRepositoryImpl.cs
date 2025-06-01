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

    public async Task<int> GetPostDislikesCount(string postId, CancellationToken cancellationToken)
    {
        return await context.Ratings
            .AsNoTracking()
            .Where(rating => rating.ArticleId == postId && rating.RatingType == RatingType.Dislike)
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetPostLikesCount(string postId, CancellationToken cancellationToken)
    {
        return await context.Ratings
            .AsNoTracking()
            .Where(rating => rating.ArticleId == postId && rating.RatingType == RatingType.Like)
            .CountAsync(cancellationToken);
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

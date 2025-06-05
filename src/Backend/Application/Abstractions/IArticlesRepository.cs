using Application.Dto.Articles;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IArticlesRepository
{
    Task<Result> CreateArticle(Article dto);
    Task<Result> ReplaceArticle(Article newArticle);
    Task<Result> PerformRatingChange(string postId, RatingType type, bool removeOld);
    Task<Result> DecrementRatingFromArtcile(string postId, RatingType type);
    Task<Article?> GetArticleById(string id);
    Task<IEnumerable<Article>> GetUserArticles(Guid userId);
    Task<Result> DeleteArtcile(string artcileId);
    Task<IEnumerable<Article>> LoadArticles(IEnumerable<string> ids);
    Task<IEnumerable<Article>> GetArticles(IEnumerable<string> tags,
        string query,
        int page,
        int pageSize,
        SortType creationDateSort,
        SortType popularitySort,
        Guid? authorId = null);
}

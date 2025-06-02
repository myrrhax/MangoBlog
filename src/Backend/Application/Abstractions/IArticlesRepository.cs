using Application.Dto.Articles;
using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;

public interface IArticlesRepository
{
    Task<Result<Article>> CreateArticle(CreateArticleDto dto);
    Task<Result<Article>> UpdateArticle(UpdateArticleDto dto);
    Task<Article?> GetArticleById(string id);
    Task<IEnumerable<Article>> GetUserArticles(Guid userId);
    Task<Result> DeleteArtcile(string artcileId);
}

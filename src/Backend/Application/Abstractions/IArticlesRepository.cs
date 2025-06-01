using Application.Dto.Articles;
using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;

public interface IArticlesRepository
{
    Task<Result<Article>> CreateArticle(CreateArticleDto dto);
    Task<Article?> GetArticleById(string id);
}

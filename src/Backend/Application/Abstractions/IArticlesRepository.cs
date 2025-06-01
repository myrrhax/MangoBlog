using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;

public interface IArticlesRepository
{
    Task<Result<Article>> AddArticle();
}

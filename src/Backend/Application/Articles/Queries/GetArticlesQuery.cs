using Application.Dto.Articles;
using MediatR;

namespace Application.Articles.Queries;

public record GetArticlesQuery(IEnumerable<string> Tags,
    string Query = "",
    string SortByDate = "",
    string SortByPopularity = "",
    Guid? AuthorId = null) : IRequest<IEnumerable<ArticleDto>>;

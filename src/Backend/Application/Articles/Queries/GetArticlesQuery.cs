using Application.Abstractions;
using Application.Dto.Articles;
using FluentValidation;
using MediatR;

namespace Application.Articles.Queries;

public record GetArticlesQuery(IEnumerable<string>? Tags = null,
    string? Query = null,
    string? SortByDate = null,
    string? SortByPopularity = null,
    Guid? AuthorId = null,
    int Page = 1) : IRequest<IEnumerable<ArticleDto>>;

public class GetArticlesQueryHandler : IRequestHandler<GetArticlesQuery, IEnumerable<ArticleDto>>
{
    private readonly IArticlesRepository _articleRepository;
    private readonly IValidator<GetArticlesQuery> _validator;

    public GetArticlesQueryHandler(IArticlesRepository articleRepository, IValidator<GetArticlesQuery> validator)
    {
        _articleRepository = articleRepository;
        _validator = validator;
    }

    public Task<IEnumerable<ArticleDto>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        
    }
}

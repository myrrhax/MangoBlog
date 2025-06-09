using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Articles.Queries;

public record GetArticlesQuery(IEnumerable<string>? Tags = null,
    string? Query = null,
    string? SortByDate = null,
    string? SortByPopularity = null,
    Guid? AuthorId = null,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<(IEnumerable<ArticleDto>, int)>>;

public class GetArticlesQueryHandler : IRequestHandler<GetArticlesQuery, Result<(IEnumerable<ArticleDto>, int)>>
{
    private readonly IArticlesRepository _articleRepository;
    private readonly IValidator<GetArticlesQuery> _validator;
    private readonly IRatingsRepository _ratingRepository;
    private readonly IUserRepository _userRepository;

    public GetArticlesQueryHandler(IArticlesRepository articleRepository, 
        IValidator<GetArticlesQuery> validator, 
        IUserRepository userRepository, 
        IRatingsRepository ratingRepository)
    {
        _articleRepository = articleRepository;
        _validator = validator;
        _userRepository = userRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task<Result<(IEnumerable<ArticleDto>, int)>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (validationResult.Errors.Any())
        {
            var validationError = new ApplicationValidationError(validationResult.Errors.ToErrorsDictionary());
            return Result.Failure<(IEnumerable<ArticleDto>, int)>(validationError);
        }

        (var articles, int count) = await _articleRepository.GetArticles(page: request.Page,
            pageSize: request.PageSize,
            query: request.Query ?? string.Empty,
            creationDateSort: StringParsing.ParseSortType(request.SortByDate ?? string.Empty),
            popularitySort: StringParsing.ParseSortType(request.SortByPopularity ?? string.Empty),
            authorId: request.AuthorId,
            tags: request.Tags ?? []);
        var auhtorIds = articles.ToDictionary(article => article.Id, article => article.CreatorId);
        Dictionary<string, ApplicationUser?> authors = await _userRepository.LoadAuthors(auhtorIds, cancellationToken);

        IEnumerable<ArticleDto> dtos = articles.Select(article =>
        {
            authors.TryGetValue(article.Id, out ApplicationUser? creator);

            return article.MapToDto(creator);
        });

        return Result.Success((dtos, count));
    }
}

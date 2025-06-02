using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Articles.Commands;

public record CreateArticleCommand(string Title, 
    Dictionary<string, dynamic> Content, 
    Guid CreatorId, 
    IEnumerable<string> Tags) : IRequest<Result<ArticleDto>>;

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, Result<ArticleDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IArticlesRepository _articlesRepository;
    private readonly ITagsRepository _tagsRepository;
    private readonly IValidator<CreateArticleCommand> _validator;

    public CreateArticleCommandHandler(IUserRepository userRepository,
        IArticlesRepository articlesRepository,
        ITagsRepository tagsRepository,
        IValidator<CreateArticleCommand> validator)
    {
        _userRepository = userRepository;
        _articlesRepository = articlesRepository;
        _tagsRepository = tagsRepository;
        _validator = validator;
    }

    public async Task<Result<ArticleDto>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (validationResult.Errors.Any())
            return Result.Failure<ArticleDto>(new ApplicationValidationError(validationResult.Errors.ToErrorsDictionary()));

        ApplicationUser? creator = await _userRepository.GetUserById(request.CreatorId, cancellationToken);
        if (creator is null)
            return Result.Failure<ArticleDto>(new UserNotFound());

        Result<IEnumerable<Tag>> tagInsertionResult = await _tagsRepository.AddTagsIfAbsent(request.Tags, cancellationToken);
        if (tagInsertionResult.IsFailure) // insertion error
            return Result.Failure<ArticleDto>(tagInsertionResult.Error);

        var article = new Article()
        {
            Title = request.Title,
            Content = request.Content,
            CreatorId = request.CreatorId,
            Tags = tagInsertionResult.Value!.ToList(),
            CreationDate = DateTime.UtcNow,
        };

        Result articleInsertionResult = await _articlesRepository.CreateArticle(article);

        return articleInsertionResult.IsSuccess
            ? Result.Success(article.MapToDto(creator))
            : Result.Failure<ArticleDto>(articleInsertionResult.Error);
    }
}
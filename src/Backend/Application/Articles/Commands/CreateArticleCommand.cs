using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Articles.Commands;

public record CreateArticleCommand(string Title, 
    Dictionary<string, dynamic> Content, 
    Guid CreatorId, 
    IEnumerable<string> Tags,
    Guid? CoverImageId = null) : IRequest<Result<ArticleDto>>;

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, Result<ArticleDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IArticlesRepository _articlesRepository;
    private readonly ITagsRepository _tagsRepository;
    private readonly IValidator<CreateArticleCommand> _validator;
    private readonly IMediaFileService _mediaFileService;

    public CreateArticleCommandHandler(IUserRepository userRepository,
        IArticlesRepository articlesRepository,
        ITagsRepository tagsRepository,
        IValidator<CreateArticleCommand> validator,
        IMediaFileService mediaFileService)
    {
        _userRepository = userRepository;
        _articlesRepository = articlesRepository;
        _tagsRepository = tagsRepository;
        _validator = validator;
        _mediaFileService = mediaFileService;
    }

    public async Task<Result<ArticleDto>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (validationResult.Errors.Any())
            return Result.Failure<ArticleDto>(new ApplicationValidationError(validationResult.Errors.ToErrorsDictionary()));

        ApplicationUser? creator = await _userRepository.GetUserById(request.CreatorId, cancellationToken);
        if (creator is null)
            return Result.Failure<ArticleDto>(new UserNotFound());

        MediaFile? file = request.CoverImageId.HasValue
            ? await _mediaFileService.GetMediaFile(request.CoverImageId.Value)
            : null;

        if (request.CoverImageId.HasValue &&
            file is { FileType: MediaFileType.Video })
        {
            return Result.Failure<ArticleDto>(new InvalidMediaFormat(request.CoverImageId.Value, MediaFileType.Video));
        }

        Result<IEnumerable<Tag>> tagInsertionResult = await _tagsRepository.AddTagsIfAbsent(request.Tags, cancellationToken);
        if (tagInsertionResult.IsFailure) // insertion error
            return Result.Failure<ArticleDto>(tagInsertionResult.Error);

        var article = new Article()
        {
            Title = request.Title,
            Content = request.Content,
            CreatorId = request.CreatorId,
            Tags = request.Tags.ToList(),
            CreationDate = DateTime.UtcNow,
            Likes = 0,
            Dislikes = 0,
            CoverImageId = request.CoverImageId,
        };

        Result articleInsertionResult = await _articlesRepository.CreateArticle(article);

        return articleInsertionResult.IsSuccess
            ? Result.Success(article.MapToDto(creator))
            : Result.Failure<ArticleDto>(articleInsertionResult.Error);
    }
}
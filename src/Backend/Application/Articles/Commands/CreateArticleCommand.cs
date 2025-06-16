using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Application.Publications.Command;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Articles.Commands;

public record CreateArticleCommand(string Title,
    Dictionary<string, dynamic> Content,
    Guid CreatorId,
    IEnumerable<string> Tags,
    Guid? CoverImageId = null,
    bool PublishToChannels = false,
    string? Caption = null) : IRequest<Result<ArticleDto>>;

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, Result<ArticleDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IArticlesRepository _articlesRepository;
    private readonly ITagsRepository _tagsRepository;
    private readonly IValidator<CreateArticleCommand> _validator;
    private readonly IMediaFileService _mediaFileService;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public CreateArticleCommandHandler(IUserRepository userRepository,
        IArticlesRepository articlesRepository,
        ITagsRepository tagsRepository,
        IValidator<CreateArticleCommand> validator,
        IMediaFileService mediaFileService,
        IPublicationsRepository publicationsRepository,
        IConfiguration configuration,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _articlesRepository = articlesRepository;
        _tagsRepository = tagsRepository;
        _validator = validator;
        _mediaFileService = mediaFileService;
        _configuration = configuration;
        _mediator = mediator;
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

        if (request.PublishToChannels
            && creator.Integration != null
            && creator.Integration.TelegramIntegration != null
            && creator.Integration.TelegramIntegration.ConnectedChannels.Any())
        {

            string clientUrl = _configuration["ClientUrl"]
                ?? throw new ArgumentNullException(nameof(clientUrl));
            IEnumerable<Guid> medias = article.CoverImageId.HasValue
                ? [article.CoverImageId.Value]
                : [];
            string content = request.Caption ?? $"У меня новая публикация на MangoBlog. Прочитать её можно здесь: {clientUrl}";

            var command = new AddPublicationCommand(content, medias, article.CreatorId, null);
            await _mediator.Send(command);
        }

        return articleInsertionResult.IsSuccess
            ? Result.Success(article.MapToDto(creator))
            : Result.Failure<ArticleDto>(articleInsertionResult.Error);
    }
}
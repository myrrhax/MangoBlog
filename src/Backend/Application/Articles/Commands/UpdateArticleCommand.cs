using System.ComponentModel.DataAnnotations;
using Application.Abstractions;
using Application.Dto.Articles;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Articles.Commands;

public record UpdateArticleCommand(string ArticleId, 
    Guid CallerId,
    string Title,
    Dictionary<string, object> Content, 
    IEnumerable<string> Tags) : IRequest<Result<ArticleDto>>;

public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, Result<ArticleDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IArticlesRepository _articlesRepository;
    private readonly IValidator<UpdateArticleCommand> _validator;
    private readonly ITagsRepository _tagsRepository;

    public UpdateArticleCommandHandler(IUserRepository userRepository, IArticlesRepository articlesRepository, IValidator<UpdateArticleCommand> validator, ITagsRepository tagsRepository)
    {
        _userRepository = userRepository;
        _articlesRepository = articlesRepository;
        _validator = validator;
        _tagsRepository = tagsRepository;
    }

    public async Task<Result<ArticleDto>> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (validationResult.Errors.Any())
        {
            var errorsDictionary = validationResult.Errors.ToErrorsDictionary();

            return Result.Failure<ArticleDto>(new ApplicationValidationError(errorsDictionary));
        }
        Task<ApplicationUser?> getUserTask = _userRepository.GetUserById(request.CallerId, cancellationToken);
        Task<Article?> getArticleTask = _articlesRepository.GetArticleById(request.ArticleId);

        await Task.WhenAll(getUserTask, getArticleTask);
        ApplicationUser? caller = getUserTask.Result;
        Article? article = getArticleTask.Result;

        if (caller is null)
            return Result.Failure<ArticleDto>(new UserNotFound());

        if (article is null)
            return Result.Failure<ArticleDto>(new ArticleNotFound(request.ArticleId));

        if (article.CreatorId != caller.Id)
            return Result.Failure<ArticleDto>(new NoPermission(caller.Id));

        Result<IEnumerable<Tag>> getTagsResult = await _tagsRepository.AddTagsIfAbsent(request.Tags, cancellationToken);

        if (getTagsResult.IsFailure)
            return Result.Failure<ArticleDto>(getTagsResult.Error);

        var dto = new UpdateArticleDto(request.ArticleId, request.Title, request.Content, getTagsResult.Value!);
        Result<Article> updateResult = await _articlesRepository.UpdateArticle(dto);
        
        if (updateResult.IsFailure)
            return Result.Failure<ArticleDto>(updateResult.Error);

        
    }
}

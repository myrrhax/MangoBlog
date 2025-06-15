﻿using System.ComponentModel.DataAnnotations;
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

public record UpdateArticleCommand(string ArticleId, 
    Guid CallerId,
    string Title,
    Dictionary<string, object> Content, 
    IEnumerable<string> Tags,
    Guid? CoverImageId = null) : IRequest<Result<ArticleDto>>;

public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, Result<ArticleDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IArticlesRepository _articlesRepository;
    private readonly IValidator<UpdateArticleCommand> _validator;
    private readonly ITagsRepository _tagsRepository;
    private readonly IMediaFileService _mediaFileService;

    public UpdateArticleCommandHandler(IUserRepository userRepository,
        IArticlesRepository articlesRepository,
        IValidator<UpdateArticleCommand> validator,
        ITagsRepository tagsRepository,
        IRatingsRepository ratingsRepository,
        IMediaFileService mediaFileService)
    {
        _userRepository = userRepository;
        _articlesRepository = articlesRepository;
        _validator = validator;
        _tagsRepository = tagsRepository;
        _mediaFileService = mediaFileService;
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

        if (request.CoverImageId.HasValue)
        {
            MediaFile? file = await _mediaFileService.GetMediaFile(request.CoverImageId.Value);
            if (file is null)
            {
                return Result.Failure<ArticleDto>(new MediaNotFound(request.CoverImageId.Value));
            }
            if (file.FileType != MediaFileType.Photo)
            {
                return Result.Failure<ArticleDto>(new InvalidMediaFormat(file.Id, file.FileType));
            }
        }

        Result<IEnumerable<Tag>> getTagsResult = await _tagsRepository.AddTagsIfAbsent(request.Tags, cancellationToken);

        if (getTagsResult.IsFailure)
            return Result.Failure<ArticleDto>(getTagsResult.Error);

        var replaceArticle = new Article(article.Id, 
            request.Title, 
            request.Content, 
            request.CallerId, 
            request.Tags.ToList(),
            article.Likes,
            article.Dislikes,
            article.CreationDate,
            request.CoverImageId);

        Result updateResult = await _articlesRepository.ReplaceArticle(replaceArticle);

        if (updateResult.IsFailure)
            return Result.Failure<ArticleDto>(updateResult.Error);

        var resultModel = new ArticleDto(replaceArticle.Id,
            caller.MapToDto(),
            replaceArticle.Title,
            replaceArticle.Content,
            replaceArticle.Tags,
            replaceArticle.CreationDate,
            replaceArticle.Likes,
            replaceArticle.Dislikes,
            UserRating: null,
            replaceArticle.CoverImageId);

        return Result.Success(resultModel);
    }
}

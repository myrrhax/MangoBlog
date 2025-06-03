using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Articles.Commands;

public record DeleteArticleCommand(string ArticleId, Guid CallerId) : IRequest<Result>;

public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, Result>
{
    private readonly IArticlesRepository _articleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRatingsRepository _ratingsRepository;

    public DeleteArticleCommandHandler(IArticlesRepository articleRepository,
        IUserRepository userRepository,
        IRatingsRepository ratingsRepository)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
        _ratingsRepository = ratingsRepository;
    }

    public async Task<Result> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var getUserTask = _userRepository.GetUserById(request.CallerId, cancellationToken);
        var getArticleTask = _articleRepository.GetArticleById(request.ArticleId);

        await Task.WhenAll(getUserTask, getArticleTask);
        ApplicationUser? user = getUserTask.Result;
        Article? article = getArticleTask.Result;

        if (user is null)
            return Result.Failure(new UserNotFound());

        if (article is null)
            return Result.Failure(new ArticleNotFound(request.ArticleId));

        if (article.CreatorId != request.CallerId)
            return Result.Failure(new NoPermission(request.CallerId));

        Result postDeleteResult = await _articleRepository.DeleteArtcile(article.Id);

        if (postDeleteResult.IsSuccess)
        {
            await _ratingsRepository.DeletePostRatings(article.Id, cancellationToken);

            return Result.Success();
        }

        return Result.Failure(postDeleteResult.Error);
    }
}

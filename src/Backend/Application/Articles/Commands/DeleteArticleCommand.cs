using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.Articles.Commands;

public record DeleteArticleCommand(string ArticleId, Guid CallerId) : IRequest<Result>;

public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, Result>
{
    private readonly IArticlesRepository _articleRepository;
    private readonly IUserRepository _userRepository;

    public DeleteArticleCommandHandler(IArticlesRepository articleRepository, IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var getUserTask = _userRepository.GetUserById(request.CallerId, cancellationToken);
        var getArticleTask = _articleRepository.GetArticleById(request.ArticleId);

        await Task.WhenAll(getUserTask, getArticleTask);
        ApplicationUser? user = getUserTask.Result;
        Article? article = getArticleTask.Result;

        if (user is null || article is null) ;
    }
}

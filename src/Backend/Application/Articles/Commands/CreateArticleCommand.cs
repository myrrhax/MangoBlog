using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Articles.Commands;

public record CreateArticleCommand(string Title, 
    Dictionary<string, object> Content, 
    Guid CreatorId, 
    IEnumerable<string> Tags) : IRequest<Result<Article>>;

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, Result<Article>>
{
    private readonly IUserRepository _userRepository;
    private readonly IArticlesRepository _articlesRepository;

    public CreateArticleCommandHandler(IUserRepository userRepository, IArticlesRepository articlesRepository)
    {
        _userRepository = userRepository;
        _articlesRepository = articlesRepository;
    }

    public async Task<Result<Article>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? creator = await _userRepository.GetUserById(request.CreatorId, cancellationToken);
        if (creator is null)
            return Result.Failure<Article>(new UserNotFound());


    }
}

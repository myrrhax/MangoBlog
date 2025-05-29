using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Users.Commands;

public record LogoutUserCommand(Guid UserId,
    string RefreshToken) : IRequest<Result>;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    public LogoutUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userRepository.GetUserById(request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure(new UserNotFound());

        RefreshToken? token = user.RefreshTokens.FirstOrDefault(token => token.Token == request.RefreshToken);
        if (token is null)
            return Result.Failure();
    }
}

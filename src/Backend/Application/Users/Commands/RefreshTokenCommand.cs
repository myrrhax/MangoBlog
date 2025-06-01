using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Users.Commands;

public record RefreshTokenCommand(string Token) : IRequest<Result<RefreshResponse>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;


    public RefreshTokenCommandHandler(IUserRepository userRepository, ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<RefreshResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        RefreshToken? refresh = await _userRepository.GetRefreshToken(request.Token, cancellationToken);

        if (refresh is null)
            return Result.Failure<RefreshResponse>(new InvalidToken());

        (string token, DateTime expiration) = _tokenGenerator.GenerateRefreshToken();
        Result updateResult = await _userRepository.UpdateRefreshToken(refresh.Id, token, expiration, cancellationToken);
        if (updateResult.IsSuccess)
        {
            string accessToken = _tokenGenerator.GenerateAccessToken(refresh.User);

            return Result.Success(new RefreshResponse(accessToken, token, refresh.User.MapToFullInfo()));
        }

        return Result.Failure<RefreshResponse>(updateResult.Error);
    }
}


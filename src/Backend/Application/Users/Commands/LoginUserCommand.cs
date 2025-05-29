using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands;

public record LoginUserCommand(string Login, string Password, string ConfirmPassword) : IRequest<Result<LoginResponse>>;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<LoginUserCommand> _validator;

    public LoginUserCommandHandler(IUserRepository userRepository, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher, IValidator<LoginUserCommand> validator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (validationResult.Errors.Any())
        {
            var validationError = new ApplicationValidationError(validationResult.Errors.ToErrorsDictionary());

            return Result.Failure<LoginResponse>(validationError);
        }
        Result<ApplicationUser> getUserResult = await _userRepository.GetUserByLogin(request.Login, cancellationToken);

        if (getUserResult.IsFailure)
            return Result.Failure<LoginResponse>();

        var writeTokenResult = await WriteTokens(user, cancellationToken);

        return writeTokenResult.IsSuccess
            ? Result.Success<RegistrationResponse>(new(writeTokenResult.Value.AccessToken, writeTokenResult.Value.RefreshToken, user.MapToFullInfo()))
            : Result.Failure<RegistrationResponse>(new UnableToWriteTokens());
    }

    private async Task<bool> VerifyIsLoginOrEmailTaken(string login, string email, CancellationToken cancellationToken)
    {
        bool isLoginTaken = await _userRepository.IsLoginTaken(login, cancellationToken);

        return isLoginTaken || await _userRepository.IsEmailTaken(email, cancellationToken);
    }

    private async Task<Result<(string AccessToken, string RefreshToken)>> WriteTokens(ApplicationUser user, CancellationToken cancellationToken)
    {
        string accessToken = _tokenGenerator.GenerateAccessToken(user);
        RefreshToken refresh = _tokenGenerator.GenerateRefreshToken(user);
        Result insertionResult = await _userRepository.AddRefreshToken(refresh, cancellationToken);

        return insertionResult.IsSuccess
            ? Result.Success((accessToken, refresh.Token))
            : Result.Failure<(string, string)>(insertionResult.Error);
    }
}

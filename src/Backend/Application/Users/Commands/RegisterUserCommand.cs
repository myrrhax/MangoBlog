using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands;

public record RegisterUserCommand(string Login, 
    string Email, 
    string Password, 
    string FirstName, 
    string LastName,
    DateOnly BirthDate,
    string? AvatarUrl = null) : IRequest<Result<RegistrationResponse>>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<RegistrationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserCommandHandler(IUserRepository userRepository, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher, IValidator<RegisterUserCommand> validator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<Result<RegistrationResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (validationResult.Errors.Any())
        {
            var validationError = new ApplicationValidationError(validationResult.Errors.ToErrorsDictionary());

            return Result.Failure<RegistrationResponse>(validationError);
        }

        bool isTaken = await VerifyIsLoginOrEmailTaken(request.Login, request.Email, cancellationToken);

        if (isTaken)
            return Result.Failure<RegistrationResponse>(new EmailOrLoginAlreadyTaken(request.Login, request.Email));

        string hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new ApplicationUser(login: request.Login, email: request.Email,
            hash: hashedPassword, firstName: request.FirstName,
            lastName: request.LastName, avatarUrl: request.AvatarUrl, birthDate: request.BirthDate);

        Result insertionResult = await _userRepository.AddUser(user, cancellationToken);
        if (insertionResult.IsFailure) 
        {
            return Result.Failure<RegistrationResponse>(insertionResult.Error);
        }

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

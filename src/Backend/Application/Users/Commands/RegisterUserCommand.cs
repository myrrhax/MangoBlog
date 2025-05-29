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
    string? AvatarUrl = null) : IRequest<Result<UserFullInfoDto>>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserFullInfoDto>>
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

    public async Task<Result<UserFullInfoDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (validationResult.Errors.Any())
        {
            var validationError = new ApplicationValidationError(validationResult.Errors.ToErrorsDictionary());

            return Result<UserFullInfoDto>.Failure(validationError);
        }

        bool isTaken = await VerifyIsLoginOrEmailTaken(request.Login, request.Email, cancellationToken);

        if (isTaken)
            return Result<UserFullInfoDto>.Failure(new EmailOrLoginAlreadyTaken(request.Login, request.Email));

        string hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new ApplicationUser(login: request.Login, email: request.Email,
            hash: hashedPassword, firstName: request.FirstName,
            lastName: request.LastName, avatarUrl: request.AvatarUrl, birthDate: request.BirthDate);

        Result insertionResult = await _userRepository.AddUser(user, cancellationToken);
        if (!insertionResult.IsSuccess) { }

    }

    private async Task<bool> VerifyIsLoginOrEmailTaken(string login, string email, CancellationToken cancellationToken)
    {
        bool isLoginTaken = await _userRepository.IsLoginTaken(login, cancellationToken);

        return isLoginTaken || await _userRepository.IsEmailTaken(email, cancellationToken);
    }
}

using Application.Abstractions;
using Application.Dto;
using Domain.Entities;
using Domain.Utils;
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

    public Task<Result<UserFullInfoDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (validationResult.Errors.Any())
        {
            return Result<RegisterUserCommand>.Failure();
        }
        
        string hashedPassword = _passwordHasher.HashPassword(request.Password);
        
        var user = new ApplicationUser(login: request.Login, email: request.Email, 
            hash: hashedPassword, firstName: request.FirstName,
            lastName: request.LastName, avatarUrl: request.AvatarUrl, birthDate: request.BirthDate);

    }
}

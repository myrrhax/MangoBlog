using Application.Users.Commands;
using FluentValidation;

namespace Application.Validation;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(command => command.Login)
            .MinimumLength(4)
            .WithMessage("Login is too short")
            .MaximumLength(15)
            .WithMessage("Login is too long")
            .Matches(RegisterUserCommandValidator.ValidLoginRegex)
            .WithMessage("Login must contain only letters of the English alphabet");

        RuleFor(command => command.Password)
            .MinimumLength(5)
            .WithMessage("Password is too short")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter of the English alphabet")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter of the English alphabet")
            .Matches("[!@#$%^&*(),.?\":{}|<>]").WithMessage("Password must contain at least one special character");
    }
}

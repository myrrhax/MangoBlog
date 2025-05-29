using Application.Users.Commands;
using FluentValidation;

namespace Application.Validation;

internal class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private const int MinAge = 3;
    private const int MaxAge = 110;
    private const string ValidLoginRegex = "^[a-zA-Z]+$";

    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.Login)
            .MinimumLength(4)
            .WithMessage("Login is too short")
            .MaximumLength(15)
            .WithMessage("Login is too long")
            .Matches(ValidLoginRegex)
            .WithMessage("Login must contain only letters of the English alphabet");

        RuleFor(command => command.Email)
            .EmailAddress()
            .WithMessage("Invalid email");

        RuleFor(command => command.AvatarUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var _))
            .When(command => command.AvatarUrl is not null)
            .WithMessage("Invalid image url");

        RuleFor(command => command.BirthDate)
            .Must(IsAgeValid)
            .WithMessage("Invalid birth date");

        RuleFor(command => command.Password)
            .MinimumLength(5)
            .WithMessage("Password is too short")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter of the English alphabet")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter of the English alphabet")
            .Matches("[!@#$%^&*(),.?\":{}|<>]").WithMessage("Password must contain at least one special character");
    }

    private bool IsAgeValid(DateOnly birthDate)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        int age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age)) // дня рождения ещё не было
            age--;

        return age >= MinAge && age <= MaxAge;
    }
}

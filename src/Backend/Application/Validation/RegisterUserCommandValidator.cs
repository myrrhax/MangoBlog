using Application.Users.Commands;
using FluentValidation;

namespace Application.Validation;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private const int MinAge = 3;
    private const int MaxAge = 110;
    private const string ValidNameFormatRegex = "^([А-Яа-яЁё]+|[A-Za-z]+)$";

    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.Login)
            .MinimumLength(4)
            .WithMessage("Login is too short")
            .MaximumLength(15)
            .WithMessage("Login is too long");

        RuleFor(command => command.Email)
            .EmailAddress()
            .WithMessage("Invalid email");

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

        RuleFor(command => command.FirstName)
            .Matches(ValidNameFormatRegex)
            .WithMessage("Invalid name format. Don't use special characters or numbers");

        RuleFor(command => command.LastName)
            .Matches(ValidNameFormatRegex)
            .WithMessage("Invalid name format. Don't use special characters or numbers");

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

using Application.Users.Commands;
using FluentValidation;

namespace Application.Validation;

public class ToogleSubscriptionCommandValidator : AbstractValidator<ToogleSubscriptionCommand>
{
    private static readonly string[] _validSubscriptionTypes = { "sub", "unsub" };
    public ToogleSubscriptionCommandValidator()
    {
        RuleFor(command => command.UserId)
            .Must(userId => userId != Guid.Empty)
            .WithMessage("Subscription channel id cannot be empty");

        RuleFor(command => command.SubscriptionType)
            .Must(type => _validSubscriptionTypes.Contains(type))
            .WithMessage($"Invalid subscription type. Valid values: [{string.Join(", ", _validSubscriptionTypes)}]");
    }
}

using Application.Integrations.Commands;
using FluentValidation;

namespace Application.Validation;

public class ConfirmPublicationCommandValidator : AbstractValidator<ConfirmPublicationCommand>
{
    private static readonly string[] validIntegrationTypes = ["tg"];
    public ConfirmPublicationCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage("User id can't be empty");

        RuleFor(command => command.PublicationId)
            .NotEmpty()
            .WithMessage("Publication id can't be empty");

        RuleFor(command => command.RoomId)
            .NotEmpty()
            .WithMessage("Room id can't be empty");

        RuleFor(command => command.MessageId)
            .NotEmpty()
            .WithMessage("Message id can't be empty");

        RuleFor(command => command.IntegrationType)
            .Must(type => validIntegrationTypes.Contains(type.ToLower().Trim()))
            .WithMessage($"Invalid integration type. Valid types: [{string.Join(", ", validIntegrationTypes)}]");
    }
}

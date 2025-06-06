﻿using Application.Integrations.Commands;
using FluentValidation;

namespace Application.Validation;

public class RemoveIntegrationCommandValidator : AbstractValidator<RemoveIntegrationCommand>
{
    private static readonly string[] validIntegrationTypes = ["tg", "vk"];

    public RemoveIntegrationCommandValidator()
    {
        RuleFor(command => command.CallerId)
            .Must(id => id != Guid.Empty)
            .WithMessage("Invalid caller id");

        RuleFor(command => command.RoomId)
            .NotEmpty()
            .WithMessage("RoomId is required");

        RuleFor(command => command.IntegrationType)
            .Must(type => validIntegrationTypes.Contains(type.ToLower().Trim()))
            .WithMessage($"Invalid integration type. Valid types: [{string.Join(", ", validIntegrationTypes)}]");
    }
}

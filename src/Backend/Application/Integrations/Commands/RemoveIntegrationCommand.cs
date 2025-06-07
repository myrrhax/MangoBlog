using Application.Abstractions;
using Application.Extentions;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Integrations.Commands;

public record RemoveIntegrationCommand(Guid CallerId, 
    string IntegrationType, 
    string RoomId) : IRequest<Result>;

public class RemoveIntegrationCommandHandler : IRequestHandler<RemoveIntegrationCommand, Result>
{
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IValidator<RemoveIntegrationCommand> _validator;

    public RemoveIntegrationCommandHandler(IIntegrationRepository integrationRepository,
        IValidator<RemoveIntegrationCommand> validator)
    {
        _integrationRepository = integrationRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(RemoveIntegrationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (validationResult.Errors.Any())
        {
            var errors = validationResult.Errors.ToErrorsDictionary();

            return Result.Failure(new ApplicationValidationError(errors));
        }

        IntegrationType type = StringParsing.ParseIntegrationType(request.IntegrationType);
        return type switch
        {
            IntegrationType.Telegram => await _integrationRepository.DeleteTelegramIntegration(request.CallerId, cancellationToken),
            _ => throw new ArgumentException(nameof(type))
        };
    }
}

using Application.Abstractions;
using Application.Extentions;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Integrations.Commands;

public record RemoveIntegrationCommand(Guid CallerId, 
    string IntegrationType) : IRequest<Result>;

public class RemoveIntegrationCommandHandler : IRequestHandler<RemoveIntegrationCommand, Result>
{
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IValidator<RemoveIntegrationCommand> _validator;
    private readonly IQueuePublisher _queuePublisher;
    private readonly IPublicationsRepository _publicationsRepository;

    public RemoveIntegrationCommandHandler(IIntegrationRepository integrationRepository,
        IValidator<RemoveIntegrationCommand> validator,
        IQueuePublisher queuePublisher,
        IPublicationsRepository publicationsRepository)
    {
        _integrationRepository = integrationRepository;
        _validator = validator;
        _queuePublisher = queuePublisher;
        _publicationsRepository = publicationsRepository;
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

    private async Task<Result> DeleteTelegramIntegration(Guid userId, CancellationToken cancellationToken)
    {
        Result deleteResult = await _integrationRepository.DeleteTelegramIntegration(userId, cancellationToken);
        if (deleteResult.IsFailure)
            return deleteResult;

        Result deleteInfoResult = await _publicationsRepository.DeleteIntegrationPublicationsInfo(userId, IntegrationType.Telegram);
        if (deleteInfoResult.IsFailure)
            return deleteInfoResult;

        return _queuePublisher.PublishDeleteIntegration(IntegrationType.Telegram, userId);
    }
}

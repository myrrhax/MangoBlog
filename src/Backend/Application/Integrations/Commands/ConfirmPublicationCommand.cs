using Application.Abstractions;
using Application.Extentions;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using FluentValidation;
using MediatR;

namespace Application.Integrations.Commands;

public record ConfirmPublicationCommand(Guid UserId,
    string PublicationId,
    string RoomId,
    string IntegrationType) : IRequest<Result>;

public class ConfirmPublicationCommandHandler : IRequestHandler<ConfirmPublicationCommand, Result>
{
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IValidator<ConfirmPublicationCommand> _validator;

    public ConfirmPublicationCommandHandler(IPublicationsRepository publicationsRepository,
        IValidator<ConfirmPublicationCommand> validator)
    {
        _publicationsRepository = publicationsRepository;
        _validator = validator;
    }

    public async Task<Result> Handle(ConfirmPublicationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (validationResult.Errors.Any())
        {
            var error = new ApplicationValidationError(validationResult.Errors.ToErrorsDictionary());

            return Result.Failure(error);
        }

        IntegrationType type = StringParsing.ParseIntegrationType(request.IntegrationType);
        Result isConfirmed = await _publicationsRepository.IsStatusUnconfirmed(request.UserId, request.PublicationId, request.RoomId, type);

        if (isConfirmed.IsFailure)
            return Result.Failure(isConfirmed.Error);

        return await _publicationsRepository.ConfirmPublicationStatus(request.PublicationId, request.RoomId, type);
    }
}

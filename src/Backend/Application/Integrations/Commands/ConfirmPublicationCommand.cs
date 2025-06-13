using Application.Abstractions;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Integrations.Commands;

public record ConfirmPublicationCommand(Guid UserId,
    string PublicationId,
    string RoomId) : IRequest<Result>;

public class ConfirmPublicationCommandHandler : IRequestHandler<ConfirmPublicationCommand, Result>
{
    private readonly IPublicationsRepository _publicationsRepository;

    public ConfirmPublicationCommandHandler(IPublicationsRepository publicationsRepository)
    {
        _publicationsRepository = publicationsRepository;
    }

    public async Task<Result> Handle(ConfirmPublicationCommand request, CancellationToken cancellationToken)
    {
        Result<bool> isConfirmed = await _publicationsRepository.GetConfirmationStatus(request.UserId, request.PublicationId, request.RoomId);

        if (isConfirmed.IsFailure)
            return Result.Failure(isConfirmed.Error);

        if (!isConfirmed.Value)
            return Result.Failure(new PublicationAlreadyConfirmed());

        return await _publicationsRepository.ConfirmPublicationStatus(request.PublicationId, request.RoomId);
    }
}

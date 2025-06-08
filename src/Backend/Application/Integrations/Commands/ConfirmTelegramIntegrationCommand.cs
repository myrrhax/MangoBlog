using Application.Abstractions;
using Application.Dto.Integrations;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.Integrations.Commands;

public record ConfirmTelegramIntegrationCommand(string IntegrationCode, 
    string TelegramId) : IRequest<Result<IntegrationDto>>;

public class ConfirmTelegramIntegrationCommandHandler : IRequestHandler<ConfirmTelegramIntegrationCommand, Result<IntegrationDto>>
{
    private readonly IIntegrationRepository _integrationRepository;

    public ConfirmTelegramIntegrationCommandHandler(IIntegrationRepository integrationRepository)
    {
        _integrationRepository = integrationRepository;
    }

    public async Task<Result<IntegrationDto>> Handle(ConfirmTelegramIntegrationCommand request, CancellationToken cancellationToken)
    {
        Result<Integration> confirmResult = await _integrationRepository.ConfirmTelegramIntegration(request.IntegrationCode, request.TelegramId, cancellationToken);
        if (confirmResult.IsFailure)
            return Result.Failure<IntegrationDto>(confirmResult.Error);

        return Result.Success(confirmResult.Value!.MapToDto());
    }
}

using Application.Abstractions;
using Application.Dto;
using Application.Dto.Integrations;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.Integrations.Commands;

public record ConfirmTelegramIntegrationCommand(string IntegrationCode, 
    string TelegramId) : IRequest<Result<ConfirmIntegrationResponse>>;

public class ConfirmTelegramIntegrationCommandHandler : IRequestHandler<ConfirmTelegramIntegrationCommand, Result<ConfirmIntegrationResponse>>
{
    private readonly IIntegrationRepository _integrationRepository;
    private readonly ITokenGenerator _tokenGenerator;

    public ConfirmTelegramIntegrationCommandHandler(IIntegrationRepository integrationRepository, ITokenGenerator tokenGenerator)
    {
        _integrationRepository = integrationRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<ConfirmIntegrationResponse>> Handle(ConfirmTelegramIntegrationCommand request, CancellationToken cancellationToken)
    {
        Result<Integration> confirmResult = await _integrationRepository.ConfirmTelegramIntegration(request.IntegrationCode, request.TelegramId, cancellationToken);
        if (confirmResult.IsFailure)
            return Result.Failure<ConfirmIntegrationResponse>(confirmResult.Error);

        Integration integration = confirmResult.Value!;
        string botToken = _tokenGenerator.GenerateIntegrationBotToken(integration.User);
        IntegrationDto dto = confirmResult.Value!.MapToDto();

        return Result.Success(new ConfirmIntegrationResponse(botToken, dto));
    }
}

using System.Security.Cryptography;
using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Integrations.Commands;

public record AddTelegramIntegrationCommand(Guid UserId) : IRequest<Result>;

public class AddTelegramIntegrationHandler : IRequestHandler<AddTelegramIntegrationCommand, Result>
{
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IUserRepository _userRepository;

    public AddTelegramIntegrationHandler(IIntegrationRepository integrationRepository,
        IUserRepository userRepository)
    {
        _integrationRepository = integrationRepository;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(AddTelegramIntegrationCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userRepository.GetUserById(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(new UserNotFound());

        TelegramIntegration? tgIntegration = await _integrationRepository.GetTelegramIntegration(request.UserId, cancellationToken);
        if (tgIntegration is not null)
            return Result.Failure(new IntegrationAlreadyExists(IntegrationType.Telegram.ToString(), request.UserId));

        Integration integration = user.Integration 
            ?? new Integration
            {
                User = user,
                UserId = user.Id,
            };

        var newTgIntegration = new TelegramIntegration()
        {
            Integration = integration,
            User = user,
            IsConfirmed = false,
            IntegrationCode = GenerateIntegrationCode()
        };

        return await _integrationRepository.AddTelegramIntegration(newTgIntegration, cancellationToken);
    }

    private string GenerateIntegrationCode()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}

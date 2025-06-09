using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.Integrations.Commands;

public record AddTelegramChatToIntegrationCommand(Guid UserId,
    string ChatId,
    string ChatName) : IRequest<Result>;

public class AddTelegramChatToIntegrationCommandHandler : IRequestHandler<AddTelegramChatToIntegrationCommand, Result>
{
    private readonly IIntegrationRepository _integrationRepository;

    public AddTelegramChatToIntegrationCommandHandler(IIntegrationRepository integrationRepository)
    {
        _integrationRepository = integrationRepository;
    }

    public async Task<Result> Handle(AddTelegramChatToIntegrationCommand request, CancellationToken cancellationToken)
    {
        var telegramChannel = new TelegramChannel()
        {
            Name = request.ChatName,
            ChannelId = request.ChatId
        };

        return await _integrationRepository.AttachTelegramChannelToIntegration(request.UserId, telegramChannel, cancellationToken);
    }
}

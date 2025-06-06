using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Integrations.Commands;

public record AddVkIntegrationCommand(Guid CallerId, 
    string ApiToken,
    string GroupId) : IRequest<Result<IntegrationDto>>;

public class AddVkIntegrationCommandHandler : IRequestHandler<AddVkIntegrationCommand, Result<IntegrationDto>>
{
    private readonly IVkApiService _tokenChecker;
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IUserRepository _userRepository;

    public AddVkIntegrationCommandHandler(IVkApiService tokenChecker,
        IIntegrationRepository integrationRepository,
        IUserRepository userRepository)
    {
        _tokenChecker = tokenChecker;
        _integrationRepository = integrationRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IntegrationDto>> Handle(AddVkIntegrationCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userRepository.GetUserById(request.CallerId, cancellationToken);
        if (user is null)
            return Result.Failure<IntegrationDto>(new UserNotFound());

        Task<Result> checkTokenResultTask = _tokenChecker.CheckGroupToken(request.ApiToken, request.GroupId);
        Task<UserIntegration?> getIntegrationInfoTask = _integrationRepository.GetIntegrationGroupId(IntegrationType.VKontakte, request.GroupId, cancellationToken);

        await Task.WhenAll(checkTokenResultTask, getIntegrationInfoTask);
        Result tokenResult = checkTokenResultTask.Result;
        UserIntegration? integrationSearch = getIntegrationInfoTask.Result;

        if (integrationSearch is not null)
            return Result.Failure<IntegrationDto>(new IntegrationAlreadyExists(request.GroupId, IntegrationType.VKontakte));

        if (tokenResult.IsFailure)
            return Result.Failure<IntegrationDto>(new InvalidApiToken());

        Integration integration = await _integrationRepository.GetIntegration(IntegrationType.VKontakte, cancellationToken);
        var userIntegration = new UserIntegration(integration,
            user,
            apiToken: request.ApiToken,
            roomId: request.GroupId,
            isConfirmed: true);
        Result insertionResult = await _integrationRepository.AddIntegration(userIntegration, cancellationToken);

        return insertionResult switch
        {
            { IsSuccess: true } => Result.Success(userIntegration.MapToDto()),
            _ => Result.Failure<IntegrationDto>(insertionResult.Error)
        };
    }
}

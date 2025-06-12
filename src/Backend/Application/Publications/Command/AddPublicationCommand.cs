using Application.Abstractions;
using Application.Dto;
using Application.Extentions;
using Domain.Entities;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Publications.Command;

public record AddPublicationCommand(string Content,
    IEnumerable<Guid> MediaIds,
    Guid UserId,
    DateTime? PublicationDate = null) : IRequest<Result<PublicationDto>>;

public class AddPublicationCommandHandler : IRequestHandler<AddPublicationCommand, Result<PublicationDto>>
{
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IQueuePublisher _publisher;
    private readonly IMediaFileService _mediaFileService;

    public AddPublicationCommandHandler(IPublicationsRepository publicationsRepository,
        IUserRepository userRepository,
        IIntegrationRepository integrationRepository,
        IQueuePublisher publisher,
        IMediaFileService mediaFileService)
    {
        _publicationsRepository = publicationsRepository;
        _userRepository = userRepository;
        _integrationRepository = integrationRepository;
        _publisher = publisher;
        _mediaFileService = mediaFileService;
    }

    public async Task<Result<PublicationDto>> Handle(AddPublicationCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userRepository.GetUserById(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<PublicationDto>(new UserNotFound());
        IEnumerable<TelegramChannel> linkedChannels = user.Integration?.TelegramIntegration
            ?.ConnectedChannels ?? [];

        if (!linkedChannels.Any())
            return Result.Failure<PublicationDto>(new NoChannlesToPublish(user.Id));

        Result<List<MediaFile>> getMediaFilesResult = await _mediaFileService.GetMediaFiles(request.MediaIds.ToList());

        if (getMediaFilesResult.IsFailure)
            return Result.Failure<PublicationDto>(new SomeMediasAreAbsent());

        var medias = getMediaFilesResult.Value!.Select(file => (file.Id, file.FileType))
            .ToList();
        List<RoomPublishStatus> statuses = linkedChannels.Select(channel => new RoomPublishStatus()
        {
            IsPublished = false,
            RoomId = channel.ChannelId,
            MessageId = null,
        }).ToList();
        var tgPublish = new IntegrationPublishInfo()
        {
            IntegrationType = Domain.Enums.IntegrationType.Telegram,
            UserId = user.Integration!.TelegramIntegration!.TelegramId!,
            PublishStatuses = statuses,
        };
        var publication = new Publication()
        {
            Content = request.Content,
            MediaFiles = medias,
            UserId = request.UserId,
            CreationDate = DateTime.UtcNow,
            PublicationTime = request.PublicationDate,
            IntegrationPublishInfos = [tgPublish]
        };

        Result insertionResult = await _publicationsRepository.AddPublication(publication);

        Dictionary<string, string>? channelNames = null;
        if (insertionResult.IsSuccess)
        {
            List<string> channelIds = statuses.Select(status => status.RoomId)
                .ToList();
            Result<Dictionary<string, string>> getChannelNamesResult = await _integrationRepository.GetChannelNamesFromIds(channelIds, cancellationToken);
            if (getChannelNamesResult.IsSuccess)
                channelNames = getChannelNamesResult.Value;
        }

        if (insertionResult.IsSuccess && publication.PublicationTime is null)
        {
            Result publishResilt = _publisher.Publish(publication);

            if (publishResilt.IsFailure)
                return Result.Failure<PublicationDto>(new FailedToPublishAMessage());
        }

        return insertionResult.IsSuccess
            ? Result.Success(publication.MapToDto(channelNames))
            : Result.Failure<PublicationDto>(insertionResult.Error);
    }
}


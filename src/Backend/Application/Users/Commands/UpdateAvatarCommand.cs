using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using Domain.Utils.Errors;
using MediatR;

namespace Application.Users.Commands;

public record UpdateAvatarCommand(Guid CallerId, Guid AvatarId) 
    : IRequest<Result>;

public class UpdateAvatarCommandHandler : IRequestHandler<UpdateAvatarCommand, Result>
{
    private readonly IMediaFileService _mediaFileService;
    private readonly IUserRepository _userRepository;

    public UpdateAvatarCommandHandler(IMediaFileService mediaFileService, IUserRepository userRepository)
    {
        _mediaFileService = mediaFileService;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        MediaFile? file = await _mediaFileService.GetMediaFile(request.AvatarId);

        return file switch
        {
            { FileType: MediaFileType.Photo, IsAvatar: true } media => await _userRepository.ChangeAvatar(request.CallerId, media.Id, cancellationToken),
            null => Result.Failure(new MediaNotFound(request.AvatarId)),
            _ => Result.Failure(new InvalidMediaFormat(file.Id, file.FileType))
        };
    }
}
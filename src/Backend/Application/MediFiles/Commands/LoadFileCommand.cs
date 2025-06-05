using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;
using MediatR;

namespace Application.MediFiles.Commands;

public record LoadFileCommand(Guid CallerId, Stream FileStream, MediaFileType FileType) 
    : IRequest<Result<MediaFile>>;

public class LoadFileCommandHandler : IRequestHandler<LoadFileCommand, Result<MediaFile>>
{
    private readonly IMediaFileService _mediaFileService;

    public LoadFileCommandHandler(IMediaFileService mediaFileService)
    {
        _mediaFileService = mediaFileService;
    }

    public async Task<Result<MediaFile>> Handle(LoadFileCommand request, CancellationToken cancellationToken)
    {
        return await _mediaFileService.LoadFileToServer(request.FileStream, request.CallerId, request.FileType);
    }
}

using Application.Abstractions;
using Domain.Entities;
using Domain.Utils;
using MediatR;

namespace Application.MediFiles.Commands;

public record LoadFileCommand(Guid CallerId, 
    Stream FileStream, 
    string FileExtention, 
    bool IsAvatar) : IRequest<Result<string>>;

public class LoadFileCommandHandler : IRequestHandler<LoadFileCommand, Result<string>>
{
    private readonly IMediaFileService _mediaFileService;

    public LoadFileCommandHandler(IMediaFileService mediaFileService)
    {
        _mediaFileService = mediaFileService;
    }

    public async Task<Result<string>> Handle(LoadFileCommand request, CancellationToken cancellationToken)
    {
        Result<MediaFile> loadingResult = await _mediaFileService.LoadFileToServer(request.FileStream, request.FileExtention, request.CallerId, request.IsAvatar);
        if (loadingResult.IsSuccess)
            return Result.Success(loadingResult.Value!.FileName);

        return Result.Failure<string>(loadingResult.Error);
    }
}

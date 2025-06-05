using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.MediFiles.Queries;

public record GetMediaByIdQuery(Guid MediaId) : IRequest<(Stream, MediaFileType)?>;

public class GetMediaByIdQueryHandler : IRequestHandler<GetMediaByIdQuery, (Stream, MediaFileType)?>
{
    private readonly IMediaFileService _mediaFileService;

    public GetMediaByIdQueryHandler(IMediaFileService mediaFileService)
    {
        _mediaFileService = mediaFileService;
    }

    public async Task<(Stream, MediaFileType)?> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        return await _mediaFileService.LoadFile(request.MediaId);
    }
}

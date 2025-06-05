using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.MediFiles.Queries;

public record GetMediaByIdQuery(Guid MediaId) : IRequest<Stream?>;

public class GetMediaByIdQueryHandler : IRequestHandler<GetMediaByIdQuery, Stream?>
{
    private readonly IMediaFileService _mediaFileService;

    public GetMediaByIdQueryHandler(IMediaFileService mediaFileService)
    {
        _mediaFileService = mediaFileService;
    }

    public async Task<Stream?> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        return await _mediaFileService.LoadFile(request.MediaId);
    }
}

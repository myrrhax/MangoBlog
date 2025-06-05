using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Infrastructure.Implementation;

internal class MediaFileServiceImpl : IMediaFileService
{
    public Task<MediaFile?> GetMediaFile(string url)
    {
        throw new NotImplementedException();
    }

    public Task<Stream?> LoadFile(string url)
    {
        throw new NotImplementedException();
    }

    public Task<Result<MediaFile>> LoadFileToServer(Stream fileStream, Guid creatorId, MediaFileType type)
    {
        throw new NotImplementedException();
    }
}

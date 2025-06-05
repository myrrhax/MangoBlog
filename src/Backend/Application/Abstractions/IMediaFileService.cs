using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IMediaFileService
{
    Task<Result<MediaFile>> LoadFileToServer(Stream fileStream, 
        string extention,
        Guid creatorId, 
        bool isAvatar);
    Task<Stream?> LoadFile(Guid fileId);
    Task<MediaFile?> GetMediaFile(Guid fileName);
}

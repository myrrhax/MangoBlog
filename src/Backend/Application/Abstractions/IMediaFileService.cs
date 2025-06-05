using Domain.Entities;
using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IMediaFileService
{
    /// <summary>
    /// Method loads file and returns hashname of file
    /// </summary>
    /// <param name="fileStream">Input file stream</param>
    /// <param name="creatorId">Creator user id</param>
    /// <param name="type">File type (video/photo)</param>
    /// <returns>File hashname</returns>
    Task<Result<MediaFile>> LoadFileToServer(Stream fileStream, Guid creatorId, MediaFileType type);
    Task<Stream?> LoadFile(string url);
    Task<MediaFile?> GetMediaFile(string url);
}

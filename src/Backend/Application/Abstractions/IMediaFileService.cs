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
    /// <param name="extention">File extentions (jpeg, mp4, e.t.c.)</param>
    /// <param name="creatorId">Creator user id</param>
    /// <param name="isAvatar">Crops image if true</param>
    /// <returns>File hashname</returns>
    Task<Result<MediaFile>> LoadFileToServer(Stream fileStream, 
        string extention,
        Guid creatorId, 
        bool isAvatar);
    Task<Stream?> LoadFile(string url);
    Task<MediaFile?> GetMediaFile(string url);
}

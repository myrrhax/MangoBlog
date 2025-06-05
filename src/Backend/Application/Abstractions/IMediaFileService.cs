using Domain.Enums;
using Domain.Utils;

namespace Application.Abstractions;

public interface IMediaFileService
{
    Task<Result<string>> LoadFileToServer(Stream fileStream, MediaFileType type);
    Task<Stream?> GetFileByHashName(string hashName);
}

using Domain.Enums;

namespace Domain.Entities;

public class MediaFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public MediaFileType FileType { get; set; }
    public DateTime LoadTime { get; set; }
    public bool IsAvatar { get; set; }
}

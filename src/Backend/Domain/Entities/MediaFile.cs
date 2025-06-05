using Domain.Enums;

namespace Domain.Entities;

public class MediaFile
{
    public Guid Id { get; set; }
    public Guid LoaderId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public MediaFileType FileType { get; set; }
    public DateTime LoadTime { get; set; }

    public ApplicationUser Loader { get; set; } = null!;
}

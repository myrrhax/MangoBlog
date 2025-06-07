using Domain.Enums;

namespace Domain.Entities;

public class Publication
{
    public string PublicationId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<Guid> MediaIds { get; set; } = [];
    public IntegrationType PublishedAt { get; set; }
    public string RoomId { get; set; } = string.Empty;
    public string PublishedPostId { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime PublicationTime { get; set; }
    public bool IsPublished { get; set; }
}

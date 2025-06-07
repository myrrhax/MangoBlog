using Domain.Enums;

namespace Domain.Entities;

public class Publication
{
    public string PublicationId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> ImagesId { get; set; } = [];
    public List<IntegrationType> PublishedAt { get; set; } = [];
    public Dictionary<IntegrationType, List<string>> PublishedPostIds { get; set; } = [];
    public DateTime CreationDate { get; set; }
    public DateTime PublicationTime { get; set; }
    public bool IsPublished { get; set; }
}

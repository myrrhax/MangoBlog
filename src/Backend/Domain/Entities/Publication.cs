using Domain.Enums;

namespace Domain.Entities;

public class Publication
{
    public string PublicationId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<Guid> MediaIds { get; set; } = [];
    public DateTime CreationDate { get; set; }
    public DateTime PublicationTime { get; set; }
    public List<IntegrationPublishInfo> IntegrationPublishInfos { get; set; } = [];
}

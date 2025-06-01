namespace Domain.Entities;

public class Article
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Dictionary<string, dynamic> Content { get; set; } = [];
    public Guid CreatorId { get; set; }
    public DateTime CreationDate { get; set; }
    public ICollection<Tag> Tags { get; set; } = [];

    public Article(string id, string title, Dictionary<string, dynamic> content, 
        Guid creatorId, ICollection<Tag> tags, DateTime? creationDate = null)
    {
        Id = id;
        Title = title;
        Content = content;
        CreatorId = creatorId;
        Tags = tags;
        CreationDate = creationDate ?? DateTime.UtcNow;
    }
}
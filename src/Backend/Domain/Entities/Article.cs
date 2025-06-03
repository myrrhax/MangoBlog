namespace Domain.Entities;

public class Article
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Dictionary<string, object> Content { get; set; } = [];
    public Guid CreatorId { get; set; }
    public DateTime CreationDate { get; set; }
    public ICollection<Tag> Tags { get; set; } = [];
    public int Likes { get; set; }
    public int Dislikes { get; set; }

    public Article(string id, string title, Dictionary<string, object> content, 
        Guid creatorId, ICollection<Tag> tags, int likes, int dislikes, DateTime? creationDate = null)
    {
        Id = id;
        Title = title;
        Content = content;
        CreatorId = creatorId;
        Tags = tags;
        CreationDate = creationDate ?? DateTime.UtcNow;
        Likes = likes;
        Dislikes = dislikes;
    }

    public Article()
    {
        
    }
}
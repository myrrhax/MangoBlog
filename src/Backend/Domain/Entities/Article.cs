namespace Domain.Entities;

public class Article
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Guid? CoverImageId { get; set; }
    public Dictionary<string, object> Content { get; set; } = [];
    public Guid CreatorId { get; set; }
    public DateTime CreationDate { get; set; }
    public ICollection<string> Tags { get; set; } = [];
    public int Likes { get; set; }
    public int Dislikes { get; set; }

    public Article(string id, string title, Dictionary<string, object> content, 
        Guid creatorId, ICollection<string> tags, int likes, int dislikes,
        DateTime? creationDate = null, Guid? coverImageId = null)
    {
        Id = id;
        Title = title;
        Content = content;
        CreatorId = creatorId;
        Tags = tags;
        CreationDate = creationDate ?? DateTime.UtcNow;
        Likes = likes;
        Dislikes = dislikes;
        CoverImageId = coverImageId;
    }

    public Article()
    {
        
    }
}
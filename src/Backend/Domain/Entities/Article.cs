namespace Domain.Entities;

public class Article
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public Dictionary<string, object> Content { get; set; } = [];
    public DateTime CreationDate { get; set; }
}

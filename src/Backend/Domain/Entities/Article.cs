namespace Domain.Entities;

public class Article
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Dictionary<string, object> Content { get; set; } = [];
    public DateTime CreationDate { get; set; }
}

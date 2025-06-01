namespace Infrastructure.Utils;

public class MongoConnectionConfig
{
    public const string ArticlesCollectionName = "Articles";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

namespace Infrastructure.Utils;

public class MongoConnectionConfig
{
    public const string ArticlesCollectionName = "Articles";
    public const string PublicationsCollectionName = "Publications";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

namespace QueryAPI.Models;
public class MongoDBSettings
{
    private string collectionName;
    private string databaseName;
    private string connectionURI;

    public string ConnectionURI { get => connectionURI; set => connectionURI = value; }
    public string DatabaseName { get => databaseName; set => databaseName = value; }
    public string CollectionName { get => collectionName; set => collectionName = value; }
}
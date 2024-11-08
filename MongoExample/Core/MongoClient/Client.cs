using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoExample.Core.MongoClient;

public class Client
{
    private readonly string _connectionString;
    private IMongoClient _client;
    
    public Client(string connectionString)
    {
        _connectionString = connectionString;
        Connect();
    }

    public void Connect()
    {
        _client = new MongoDB.Driver.MongoClient(_connectionString);
    }

    public IMongoCollection<T> Collection<T>(string databaseName, string collectionName)
    {
        return _client.GetDatabase(databaseName).GetCollection<T>(collectionName);
    }
}
using MongoDB.Driver;

namespace EduAssign.API.Data;

public class MongoDbContext
{
private readonly IMongoDatabase _database;

public IMongoDatabase Database => _database;

public MongoDbContext(IConfiguration configuration)
{
    var connectionString =
        configuration["MongoDb:ConnectionString"];

    var databaseName =
        configuration["MongoDb:DatabaseName"];

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "MongoDB connection string is missing."
        );
    }

    if (string.IsNullOrWhiteSpace(databaseName))
    {
        throw new InvalidOperationException(
            "MongoDB database name is missing."
        );
    }

    var client = new MongoClient(connectionString);

    _database = client.GetDatabase(databaseName);
}

public IMongoCollection<T> GetCollection<T>(string name)
{
    return _database.GetCollection<T>(name);
}

}
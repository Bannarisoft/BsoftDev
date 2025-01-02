using Core.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
namespace BSOFT.Infrastructure.Data
{
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("MongoDbConnectionString");
        var client = new MongoClient(mongoConnectionString);
        _database = client.GetDatabase(configuration["MongoDb:DatabaseName"]);
    }

    // Get MongoDB collection for AuditLogs
    public IMongoCollection<AuditLogs> AuditLogs => _database.GetCollection<AuditLogs>("AuditLogs");

    // You can ensure the collection is created by inserting the first document or checking for the collection explicitly
    public void EnsureCollectionExists()
    {
        var collectionNames = _database.ListCollectionNames().ToList();
        if (!collectionNames.Contains("AuditLogs"))
        {
            _database.CreateCollection("AuditLogs");  // Create the collection if it does not exist
        }
    }
}
}



using MongoDB.Driver;

namespace SagaOrchestrator.Application.Common.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MongoDB.Driver;

namespace BSOFT.Infrastructure.Repositories
{
    public class AuditLogMongoRepository : IAuditLogRepository
    {
        private readonly IMongoCollection<AuditLogs> _auditLogsCollection;
         private readonly MongoDbContext _mongoDbContext;

        public AuditLogMongoRepository(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
           _auditLogsCollection = _mongoDbContext.AuditLogs;            
        }

        public async Task<AuditLogs> CreateAsync(AuditLogs auditLog)
        {
            if (auditLog == null) throw new ArgumentNullException(nameof(auditLog));

            try
            {
                _mongoDbContext.UpdateAuditFields(auditLog);
                await _auditLogsCollection.InsertOneAsync(auditLog);
                return auditLog;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting audit log: {ex.Message}");
                throw;
            }
        }

        public async Task<List<AuditLogs>> GetAllAsync()
        {
            try
            {
                return await _auditLogsCollection.Find(_ => true).ToListAsync() ?? new List<AuditLogs>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all audit logs: {ex.Message}");
                return new List<AuditLogs>();
            }
        }  
    public async Task<List<AuditLogs>> GetByAuditLogNameAsync(string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("Search pattern cannot be null or empty.", nameof(searchPattern));
            }

            try
            {
                var filter = Builders<AuditLogs>.Filter.Or(
                    Builders<AuditLogs>.Filter.Regex("UserName", new MongoDB.Bson.BsonRegularExpression(searchPattern, "i")),
                    Builders<AuditLogs>.Filter.Regex("Action", new MongoDB.Bson.BsonRegularExpression(searchPattern, "i")),
                    Builders<AuditLogs>.Filter.Regex("Details", new MongoDB.Bson.BsonRegularExpression(searchPattern, "i"))
                );

                return await _auditLogsCollection.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching audit logs by name: {ex.Message}");
                throw;
            }
        }
    }
}

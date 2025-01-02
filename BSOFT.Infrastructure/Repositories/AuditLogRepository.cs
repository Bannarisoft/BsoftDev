using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class AuditLogMongoRepository : IAuditLogRepository
    {
        private readonly IMongoCollection<AuditLogs> _auditLogsCollection;

        public AuditLogMongoRepository(IMongoDatabase database)
        {
            _auditLogsCollection = database.GetCollection<AuditLogs>("AuditLogs");
        }

        public async Task<AuditLogs> CreateAsync(AuditLogs auditLog)
        {
            await _auditLogsCollection.InsertOneAsync(auditLog);
            return auditLog;
        }
        
        public async Task<List<AuditLogs>> GetAllAsync()
        {
            return await _auditLogsCollection.Find(_ => true).ToListAsync();
        }        
        public async Task<List<AuditLogs>> GetByAuditLogNameAsync(string searchPattern)
        {
            var filter = Builders<AuditLogs>.Filter.Or(
                Builders<AuditLogs>.Filter.Regex("UserName", new MongoDB.Bson.BsonRegularExpression(searchPattern, "i")),
                Builders<AuditLogs>.Filter.Regex("Action", new MongoDB.Bson.BsonRegularExpression(searchPattern, "i")),
                Builders<AuditLogs>.Filter.Regex("Details", new MongoDB.Bson.BsonRegularExpression(searchPattern, "i"))
            );
            return await _auditLogsCollection.Find(filter).ToListAsync();
        }

/*         public async Task UpdateAsync(AuditLogs auditLog)
        {
            var filter = Builders<AuditLogs>.Filter.Eq(a => a.Id, auditLog.Id);
            await _auditLogsCollection.ReplaceOneAsync(filter, auditLog);
        } */
    }
}

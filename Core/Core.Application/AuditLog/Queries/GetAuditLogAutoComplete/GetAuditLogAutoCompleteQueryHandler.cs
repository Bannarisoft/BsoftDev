using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using Core.Application.AuditLog.Queries;
using MediatR;
using Core.Application.AuditLog.Queries.GetAuditLog;

namespace Core.Application.AuditLog.Queries.GetAuditLogBySearchPattern
{
    public class GetAuditLogBySearchPatternQueryHandler : IRequestHandler<GetAuditLogBySearchPatternQuery, List<AuditLogDto>>
    {
        private readonly IMongoCollection<AuditLogs> _auditLogCollection;

        public GetAuditLogBySearchPatternQueryHandler(IMongoDatabase mongoDatabase)
        {
            _auditLogCollection = mongoDatabase.GetCollection<AuditLogs>("AuditLogs");
        }

        public async Task<List<AuditLogDto>> Handle(GetAuditLogBySearchPatternQuery request, CancellationToken cancellationToken)
        {
            // Create a case-insensitive regex filter for searching based on the SearchPattern
            var filter = Builders<AuditLogs>.Filter.Or(
    Builders<AuditLogs>.Filter.Regex("UserName", new MongoDB.Bson.BsonRegularExpression(request.SearchPattern, "i")),
    Builders<AuditLogs>.Filter.Regex("Action", new MongoDB.Bson.BsonRegularExpression(request.SearchPattern, "i")),
    Builders<AuditLogs>.Filter.Regex("Details", new MongoDB.Bson.BsonRegularExpression(request.SearchPattern, "i"))
);

            // Retrieve matching audit logs
            var auditLogs = await _auditLogCollection.Find(filter).ToListAsync(cancellationToken);

            // Convert audit logs to DTOs
            var auditLogDto = auditLogs.Select(log => new AuditLogDto
            {      
                Id = log.Id.ToString(),          
                UserId = log.UserId,
                UserName = log.UserName,
                IPAddress = log.IPAddress,
                OS = log.OS,
                Browser = log.Browser,
                Action = log.Action,
                Details = log.Details,
                Module = log.Module,
                CreatedAt = log.CreatedAt                
            }).ToList();

            return auditLogDto;
        }
    }
}

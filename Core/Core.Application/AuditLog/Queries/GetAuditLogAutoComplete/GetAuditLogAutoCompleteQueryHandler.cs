using MongoDB.Driver;
using Core.Domain.Entities;

using MediatR;
using Core.Application.AuditLog.Queries.GetAuditLog;
using Core.Application.Common;

namespace Core.Application.AuditLog.Queries.GetAuditLogBySearchPattern
{
    public class GetAuditLogBySearchPatternQueryHandler : IRequestHandler<GetAuditLogBySearchPatternQuery, Result<List<AuditLogDto>>>
    {
        private readonly IMongoCollection<AuditLogs> _auditLogCollection;

        public GetAuditLogBySearchPatternQueryHandler(IMongoDatabase mongoDatabase)
        {
            _auditLogCollection = mongoDatabase.GetCollection<AuditLogs>("AuditLogs");
        }

        public async Task<Result<List<AuditLogDto>>> Handle(GetAuditLogBySearchPatternQuery request, CancellationToken cancellationToken)
        {
            // Create a case-insensitive regex filter for searching based on the SearchPattern
            var filter = Builders<AuditLogs>.Filter.Or(
                Builders<AuditLogs>.Filter.Regex("UserName", new MongoDB.Bson.BsonRegularExpression(request.SearchPattern, "i")),
                Builders<AuditLogs>.Filter.Regex("Action", new MongoDB.Bson.BsonRegularExpression(request.SearchPattern, "i")),
                Builders<AuditLogs>.Filter.Regex("Details", new MongoDB.Bson.BsonRegularExpression(request.SearchPattern, "i"))
                );
            try
            {            
                var auditLogs = await _auditLogCollection.Find(filter).ToListAsync(cancellationToken);
         // Check if auditLogs is null or empty
                if (auditLogs == null || !auditLogs.Any())
                {
                    return Result<List<AuditLogDto>>.Failure("No audit logs found matching the search pattern.");
                }
                var auditLogDto = auditLogs.Select(log => new AuditLogDto
                {      
                    Id = log.Id.ToString(),          
                    CreatedBy = log.CreatedBy,
                    CreatedByName = log.CreatedByName,
                    IPAddress = log.IPAddress,
                    OS = log.OS,
                    Browser = log.Browser,
                    Action = log.Action,
                    Details = log.Details,
                    Module = log.Module,
                    CreatedAt = log.CreatedAt ,
                    MachineName = log.MachineName
                }).ToList();

            return Result<List<AuditLogDto>>.Success(auditLogDto);
            }
            catch (Exception ex)
            {
                // Wrap the error into a failure response
                return Result<List<AuditLogDto>>.Failure($"Error retrieving audit logs: {ex.Message}");
            }
        }
    }
}

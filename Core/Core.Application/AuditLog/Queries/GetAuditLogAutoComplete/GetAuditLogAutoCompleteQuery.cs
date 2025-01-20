using Core.Application.AuditLog.Queries.GetAuditLog;
using Core.Application.Common;
using MediatR;
using System.Collections.Generic;

namespace Core.Application.AuditLog.Queries
{
    public class GetAuditLogBySearchPatternQuery : IRequest<Result<List<AuditLogDto>>>
    {
        public string? SearchPattern { get; set; } 
    }
}

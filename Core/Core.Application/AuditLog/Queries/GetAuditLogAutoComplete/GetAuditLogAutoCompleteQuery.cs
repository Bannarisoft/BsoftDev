using Core.Application.AuditLog.Queries.GetAuditLog;
using MediatR;
using System.Collections.Generic;

namespace Core.Application.AuditLog.Queries
{
    public class GetAuditLogBySearchPatternQuery : IRequest<List<AuditLogDto>>
    {
        public string SearchPattern { get; set; } = string.Empty;
    }
}

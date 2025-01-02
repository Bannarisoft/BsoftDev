using Core.Application.AuditLog.Queries.GetAuditLog;
using Core.Application.Common;
using MediatR;

namespace Core.Application.AuditLog.Commands.CreateAuditLog
{
    public class CreateAuditLogCommand : IRequest<Result<AuditLogDto>>
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
    }
}

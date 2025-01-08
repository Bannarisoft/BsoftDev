using Core.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BSOFT.Infrastructure.Repositories
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor
        public AuditLogService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetUserIPAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
        }

        public string GetUserAgent()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown User-Agent";
        }

        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ?? "Anonymous";
        }
    }


}

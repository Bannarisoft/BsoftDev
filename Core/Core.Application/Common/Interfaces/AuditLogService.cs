namespace Core.Application.Common.Interfaces
{
    public interface IAuditLogService
    {
        string GetUserIPAddress();
        string GetUserAgent();
        string GetCurrentUserId();
    }
}
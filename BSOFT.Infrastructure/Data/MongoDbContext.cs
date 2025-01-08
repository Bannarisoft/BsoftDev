using Core.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using Core.Application.Common.Interfaces;

namespace BSOFT.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;

        public MongoDbContext(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var mongoConnectionString = configuration.GetConnectionString("MongoDbConnectionString");
            var client = new MongoClient(mongoConnectionString);
            _database = client.GetDatabase(configuration["MongoDb:DatabaseName"]);
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
        }

        public IMongoCollection<AuditLogs> AuditLogs => _database.GetCollection<AuditLogs>("AuditLogs");

        public void EnsureCollectionExists()
        {
            try
            {
                var collectionNames = _database.ListCollectionNames().ToList();
                if (!collectionNames.Contains("AuditLogs"))
                {
                    _database.CreateCollection("AuditLogs");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring collection exists: {ex.Message}");
                throw;
            }
        }
         /*  // Insert with audit field updates
            public async Task InsertWithAuditAsync(AuditLogs auditLog)
            {
                UpdateAuditFields(auditLog);  // Ensure audit fields are set before insertion
                await AuditLogs.InsertOneAsync(auditLog);  // Insert the audit log into MongoDB
            } */
    
        public void UpdateAuditFields(AuditLogs auditLog)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? _auditLogService.GetUserIPAddress() ?? "Unknown IP";
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

            auditLog.MachineName = Environment.MachineName;
            auditLog.IPAddress = ipAddress;
            auditLog.OS = userAgent != null ? GetOperatingSystem(userAgent) : "Unknown OS";
            auditLog.Browser = userAgent != null ? GetBrowser(userAgent) : "Unknown Browser";
            auditLog.CreatedAt = DateTime.UtcNow;
        }

        private string GetOperatingSystem(string userAgent)
        {
            if (userAgent.Contains("Windows")) return "Windows";
            if (userAgent.Contains("Mac OS")) return "Mac OS";
            if (userAgent.Contains("Linux")) return "Linux";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone")) return "iOS";
            return "Unknown OS";
        }

        private string GetBrowser(string userAgent)
        {
            if (userAgent.Contains("Firefox")) return "Firefox";
            if (userAgent.Contains("Chrome")) return "Chrome";
            if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) return "Safari";
            if (userAgent.Contains("Edge")) return "Edge";
            if (userAgent.Contains("Opera") || userAgent.Contains("OPR")) return "Opera";
            return "Unknown Browser";
        }
    }
}

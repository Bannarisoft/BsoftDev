using System.Data;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationConfig;
using Core.Application.Common.Interfaces;
using Dapper;

namespace  BackgroundService.Infrastructure.Repositories.Notification.NotificationConfig
{
    public class NotificationConfigQueryRepository : INotificationConfigQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;

        public NotificationConfigQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<Domain.Entities.Notification.NotificationConfig?> GetByIdAsync(int Id)
        {
            const string query = @"
                    SELECT Id, ModuleName, NotificationEventTypeId,  IsActive, IsDeleted, CreatedBy, CreatedDate, CreatedByName, CreatedIP, ModifiedBy, ModifiedDate, ModifiedByName, ModifiedIP
                    FROM  AppNotification.NotificationConfig
                    WHERE Id = @Id AND IsDeleted = 0";

            var notificationConfig = await _dbConnection.QueryFirstOrDefaultAsync<Domain.Entities.Notification.NotificationConfig>(query, new { Id });
            return notificationConfig;
        }

        public async Task<List<Domain.Entities.Notification.NotificationConfig>> GetNotificationConfigAutoCompleteAsync(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty;
            const string query = @"
             SELECT Id, ModuleName 
            FROM AppNotification.NotificationConfig
            WHERE IsDeleted = 0 
            AND ModuleName LIKE @SearchPattern";
            var parameters = new
            {
                SearchPattern = $"%{searchPattern}%"
            };
            var notificationConfig = await _dbConnection.QueryAsync<Domain.Entities.Notification.NotificationConfig>(query, parameters);
            return notificationConfig.ToList();
        }

        public async Task<(IEnumerable<dynamic>, int)> GetAllNotificationConfigAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
            FROM AppNotification.NotificationConfig
            WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ModuleName LIKE @Search)")}};

            SELECT 
            Id, ModuleName, NotificationEventTypeId,  IsActive, IsDeleted, CreatedBy, CreatedDate, CreatedByName, CreatedIP, ModifiedBy, ModifiedDate, ModifiedByName, ModifiedIP
            FROM  AppNotification.NotificationConfig 
            WHERE 
            IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ModuleName LIKE @Search )")}}
            ORDER BY Id desc
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT @TotalCount AS TotalCount;
            """;

            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var notificationConfig = await _dbConnection.QueryMultipleAsync(query, parameters);
            var notificationConfigList = (await notificationConfig.ReadAsync<Domain.Entities.Notification.NotificationConfig>()).ToList();
            int totalCount = (await notificationConfig.ReadFirstAsync<int>());
            return (notificationConfigList, totalCount);
        }
        public async Task<bool> SoftDeleteValidation(int Id)
        {
            const string query = @"
                    SELECT 1 
                    FROM AppNotification.NotificationConfig
                    WHERE id = @Id AND IsDeleted = 0;";
            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });
            var notificationConfigExists = await multi.ReadFirstOrDefaultAsync<int?>();
            return notificationConfigExists.HasValue;
        }
        public async Task<bool> NotFoundAsync(int Id)
        {
            var query = "SELECT COUNT(1) FROM AppNotification.NotificationConfig WHERE Id = @Id AND IsDeleted = 0";             
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = Id });
            return count > 0;
        }   
    }
}
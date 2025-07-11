using System.Data;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Queries.GetAllNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Queries.GetNotificationConfigAutoComplete;
using Dapper;

namespace  BackgroundService.Infrastructure.Repositories.Notification.NotificationConfig
{
    public class NotificationConfigQueryRepository : INotificationConfigQueryRepository
    {
        private readonly IDbConnection _dbConnection;       

        public NotificationConfigQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;            
        }

        public async Task<NotificationConfigDto> GetByIdAsync(int Id)
        {
            const string query = @" select 
                    NC.Id, ModuleName, NotificationEventTypeId, NC.IsActive, NC.IsDeleted, NC.CreatedBy, NC.CreatedDate, NC.CreatedByName, NC.CreatedIP, NC.ModifiedBy, NC.ModifiedDate, NC.ModifiedByName, NC.ModifiedIP,MM.Code
                    FROM  AppNotification.NotificationConfig NC
                    INNER JOIN AppData.MiscMaster  MM on MM.id=NC.NotificationEventTypeId
                    WHERE NC.Id = @Id AND NC.IsDeleted = 0";

            var notificationConfig = await _dbConnection.QueryFirstOrDefaultAsync<NotificationConfigDto>(query, new { Id });
            return notificationConfig;
        }

        public async Task<List<NotificationConfigAutoCompleteDto>> GetNotificationConfigAutoCompleteAsync(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty;
            const string query = @"
             SELECT NC.Id, NC.ModuleName 
            FROM AppNotification.NotificationConfig NC            
            WHERE NC.IsDeleted = 0 
            AND ModuleName LIKE @SearchPattern";
            var parameters = new
            {
                SearchPattern = $"%{searchPattern}%"
            };
            var notificationConfig = await _dbConnection.QueryAsync<NotificationConfigAutoCompleteDto>(query, parameters);
            return notificationConfig.ToList();
        }

        public async Task<(IEnumerable<dynamic>, int)> GetAllNotificationConfigAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
            FROM AppNotification.NotificationConfig
            WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ModuleName LIKE @Search)")}};

            SELECT 
            NC.Id, ModuleName, NotificationEventTypeId, NC.IsActive, NC.IsDeleted, NC.CreatedBy, NC.CreatedDate, NC.CreatedByName, NC.CreatedIP, NC.ModifiedBy, NC.ModifiedDate, NC.ModifiedByName, NC.ModifiedIP,MM.Code
            FROM  AppNotification.NotificationConfig NC
            INNER JOIN AppData.MiscMaster  MM on MM.id=NC.NotificationEventTypeId
            WHERE 
            NC.IsDeleted = 0
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
            var notificationConfigList = (await notificationConfig.ReadAsync<NotificationConfigDto>()).ToList();
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces.Notification.INotificationGroup;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Notification.NotificationGroup
{
    public class NotificationGroupQueryRepository : INotificationGroupQuery
    {
        private readonly IDbConnection _dbConnection;
        public NotificationGroupQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<(List<Domain.Entities.Notification.NotificationGroup>, int)> GetAllNotificationGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM [AppNotification].[NotificationGroup]
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (GroupName LIKE @Search)")}};

                SELECT 
                Id, 
                GroupName,
                IsActive,CreatedDate,CreatedBy,CreatedByName,ModifiedBy,ModifiedDate,ModifiedByName
            FROM [AppNotification].[NotificationGroup]
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (GroupName LIKE @Search )")}}
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

            var NotificationGroup = await _dbConnection.QueryMultipleAsync(query, parameters);
            var NotificationGrouplist = (await NotificationGroup.ReadAsync<Domain.Entities.Notification.NotificationGroup>()).ToList();
            int totalCount = await NotificationGroup.ReadFirstAsync<int>();

            return (NotificationGrouplist, totalCount);
        }

        public async Task<List<Domain.Entities.Notification.NotificationGroup>> GetNotificationGroupsAutoComplete(string searchPattern)
        {
              const string query = @"
                SELECT Id, GroupName 
                FROM [AppNotification].[NotificationGroup] 
                WHERE IsDeleted = 0 AND IsActive=1 AND GroupName LIKE @SearchPattern";
                
            var NotificationGroups = await _dbConnection.QueryAsync<Domain.Entities.Notification.NotificationGroup>(query, new { SearchPattern = $"%{searchPattern}%" });
            return NotificationGroups.ToList();
        }
    }
}
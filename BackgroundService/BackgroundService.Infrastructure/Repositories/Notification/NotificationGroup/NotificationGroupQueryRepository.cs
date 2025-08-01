using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroup;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Notification.NotificationGroup
{
    public class NotificationGroupQueryRepository : INotificationGroupQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        public NotificationGroupQueryRepository(IDbConnection dbConnection, IIPAddressService iPAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = iPAddressService;
        }

        public async Task<bool> AlreadyExistsAsync(string GroupName, int? id = null)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = "SELECT COUNT(1) FROM [AppNotification].[NotificationGroup] WHERE GroupName = @GroupName AND IsDeleted = 0 AND UnitId=@UnitId";
            var parameters = new DynamicParameters(new { GroupName,UnitId });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
        }

        public async Task<(List<Domain.Entities.Notification.NotificationGroup>, int)> GetAllNotificationGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();
             var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM [AppNotification].[NotificationGroup]
              WHERE UnitId=@UnitId AND IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (GroupName LIKE @Search)")}};

                SELECT 
                Id, 
                GroupName,
                IsActive,CreatedDate,CreatedBy,CreatedByName,ModifiedBy,ModifiedDate,ModifiedByName
            FROM [AppNotification].[NotificationGroup]
            WHERE UnitId=@UnitId AND  IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (GroupName LIKE @Search )")}}
                ORDER BY Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            """;


            var parameters = new
            {
                UnitId,
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
            var UnitId = _ipAddressService.GetUnitId();
              const string query = @"
                SELECT Id, GroupName 
                FROM [AppNotification].[NotificationGroup] 
                WHERE UnitId=@UnitId AND  IsDeleted = 0 AND IsActive=1 AND GroupName LIKE @SearchPattern";
                
            var NotificationGroups = await _dbConnection.QueryAsync<Domain.Entities.Notification.NotificationGroup>(query, new { UnitId,SearchPattern = $"%{searchPattern}%" });
            return NotificationGroups.ToList();
        }

        public async Task<bool> NotFoundAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM [AppNotification].[NotificationGroup] WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
        }
    }
}
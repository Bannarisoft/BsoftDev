using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Application.Notification.NotificationGroupMember.Queries.GetAllNotificationGroupMember;
using BackgroundService.Domain.Entities.Notification;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Notification.NotificationGroupMember
{
    public class NotificationGroupMemberQueryRepository : INotificationGroupMemberQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        public NotificationGroupMemberQueryRepository(IDbConnection dbConnection, IIPAddressService iPAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = iPAddressService;
        }
        public async Task<bool> AlreadyExistsAsync(int GroupId, int UserId, int? id = null)
        {
                var query = @"SELECT COUNT(1) 
                          FROM [AppNotification].[NotificationGroupMembers]
                          WHERE GroupId = @GroupId AND UserId = @UserId AND IsDeleted = 0";

                var parameters = new DynamicParameters(new { GroupId, UserId });

                if (id is not null)
                {
                    query += " AND Id != @Id";
                    parameters.Add("Id", id);
                }

                var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
        }

      public async Task<(List<NotificationGroupMemberDto>, int)> GetAllNotificationGroupAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();
            const string dataQuery = @"
                SELECT  
                    NGM.Id AS Id, 
                    NGM.GroupId,
                    NGM.UserId,
                    U.UserName,
                    NG.GroupName,
                    NGM.IsActive, 
                    NGM.CreatedBy, 
                    NGM.CreatedDate, 
                    NGM.CreatedByName, 
                    NGM.ModifiedBy, 
                    NGM.ModifiedDate, 
                    NGM.ModifiedByName
                FROM [AppNotification].[NotificationGroupMembers] NGM
                INNER JOIN [AppNotification].[NotificationGroup] NG ON NG.Id = NGM.GroupId
                LEFT JOIN Bannari.AppSecurity.Users U ON U.UserId = NGM.UserId
                WHERE NG.UnitId=@UnitId AND  NGM.IsDeleted = 0 and NGM.IsActive = 1 and NG.IsActive = 1
                AND (@Search IS NULL OR NG.GroupName LIKE @Search)
                ORDER BY NG.GroupName
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";

            const string countQuery = @"
                SELECT COUNT(*) 
                FROM [AppNotification].[NotificationGroupMembers] NGM
                INNER JOIN [AppNotification].[NotificationGroup] NG ON NG.Id = NGM.GroupId
                WHERE  NG.UnitId=@UnitId AND  NGM.IsDeleted = 0 
                AND (@Search IS NULL OR NG.GroupName LIKE @Search);
            ";

            var parameters = new
            {
                UnitId,
                Search = string.IsNullOrEmpty(searchTerm) ? null : $"%{searchTerm}%",
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            var result = await _dbConnection.QueryAsync<NotificationGroupMemberDto>(
                dataQuery,
                parameters
            );

            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

            return (result.ToList(), totalCount);
        }



        public async Task<bool> NotFoundAsync(int groupId)
        {
            var query = "SELECT COUNT(1) FROM [AppNotification].[NotificationGroupMembers] WHERE GroupId = @Id AND IsDeleted = 0";
             
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = groupId });
            return count > 0;
        }
    }
}
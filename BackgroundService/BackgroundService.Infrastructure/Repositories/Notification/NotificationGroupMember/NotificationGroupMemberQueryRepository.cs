using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Domain.Entities.Notification;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Notification.NotificationGroupMember
{
    public class NotificationGroupMemberQueryRepository : INotificationGroupMemberQuery
    {
        private readonly IDbConnection _dbConnection;
        public NotificationGroupMemberQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AlreadyExistsAsync(int GroupId, int UserId, int? id = null)
        {
               var query = "SELECT COUNT(1) FROM [AppNotification].[NotificationGroupMembers] WHERE GroupId = @GroupId AND UserId = @UserId AND IsDeleted = 0";
                var parameters = new DynamicParameters(new { GroupId,UserId });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
        }

        public async Task<(List<NotificationGroupMembers>, int)> GetAllNotificationGroupAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            const string dataQuery = @"
              SELECT  
                  NGM.Id, NGM.GroupId, NGM.UserId, NGM.IsActive, NGM.CreatedBy, NGM.CreatedDate, 
                  NGM.CreatedByName, NGM.ModifiedBy, NGM.ModifiedDate, NGM.ModifiedByName,
                  NG.Id, NG.GroupName
              FROM [AppNotification].[NotificationGroupMembers] NGM
              INNER JOIN [AppNotification].[NotificationGroup] NG ON NG.Id = NGM.GroupId
              WHERE NG.IsDeleted = 0
                AND (@Search IS NULL OR NG.GroupName LIKE @Search)
              ORDER BY NG.GroupName
              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
          ";

          const string countQuery = @"
              SELECT COUNT(*) 
               FROM [AppNotification].[NotificationGroupMembers] NGM
              INNER JOIN [AppNotification].[NotificationGroup] NG ON NG.Id = NGM.GroupId
              WHERE NGM.IsDeleted = 0 
                AND (@Search IS NULL OR GroupName LIKE @Search);
          ";

          var parameters = new
          {
              Search = string.IsNullOrEmpty(searchTerm) ? null : $"%{searchTerm}%",
              Offset = (pageNumber - 1) * pageSize,
              PageSize = pageSize
          };

          var result = await _dbConnection.QueryAsync<NotificationGroupMembers, Domain.Entities.Notification.NotificationGroup, NotificationGroupMembers>(
              dataQuery,
              (member, group) =>
              {
                  member.Group = group;
                  return member;
              },
              param: parameters,
              splitOn: "Id"
          );

          var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

          return (result.ToList(), totalCount);
            
        }


        public async Task<bool> NotFoundAsync(int id)
        {
             var query = "SELECT COUNT(1) FROM [AppNotification].[NotificationGroupMembers] WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
        }
    }
}
using System.Data;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Domain.Entities.Notification;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Notification.NotificationEventRules
{
    public class NotificationEventRuleQueryRepository : INotificationEventRuleQuery
    {
        private readonly IDbConnection _dbConnection;
        public NotificationEventRuleQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AlreadyExistsAsync(int NotificationChannelId, int TemplateId, int NotificationLevelHierarchyId, int RecipientTypeId, int? id = null)
        {
            var query = @"SELECT COUNT(1) 
                        FROM [AppNotification].[NotificationEventRule] 
                        WHERE NotificationChannelId = @NotificationChannelId AND 
                        TemplateId=@TemplateId AND NotificationLevelHierarchyId = @NotificationLevelHierarchyId
                        AND RecipientTypeId = @RecipientTypeId AND IsDeleted = 0";
                var parameters = new DynamicParameters(new { NotificationChannelId, TemplateId, NotificationLevelHierarchyId, RecipientTypeId });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
        }

        public async Task<(List<NotificationEventRule>, int)> GetAllNotificationGroupAsync(int pageNumber, int pageSize, string? searchTerm)
        {
             const string dataQuery = @"
              SELECT  
                  NER.Id, NER.NotificationChannelId, NER.TemplateId, NER.NotificationLevelHierarchyId,
                  NER.RecipientTypeId,NER.IsActive, NER.CreatedBy,
                    NER.CreatedDate,NER.CreatedByName, NER.ModifiedBy, NER.ModifiedDate, NER.ModifiedByName,
                    NotificationChannel.Id,NotificationChannel.Code,RecipientType.Id,RecipientType.Code
              FROM [AppNotification].[NotificationEventRule] NER
              INNER JOIN AppNotification.NotificationLevelHierarchy NH on NH.Id=NER.NotificationLevelHierarchyId
              INNER JOIN AppNotification.NotificationTemplate NT on NT.Id=NER.TemplateId
              INNER JOIN AppData.MiscMaster NotificationChannel on NotificationChannel.Id=NER.NotificationChannelId
              INNER JOIN AppData.MiscMaster RecipientType on RecipientType.Id=NER.RecipientTypeId
              WHERE NER.IsDeleted = 0
                       AND (
              @Search IS NULL OR 
              NotificationChannel.Code LIKE '%' + @Search + '%' OR 
              RecipientType.Code LIKE '%' + @Search + '%'
                )
              ORDER BY NER.Id
              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
          ";

          const string countQuery = @"
              SELECT COUNT(*) 
               FROM [AppNotification].[NotificationEventRule] NER
              INNER JOIN AppNotification.NotificationLevelHierarchy NH on NH.Id=NER.NotificationLevelHierarchyId
              INNER JOIN AppNotification.NotificationTemplate NT on NT.Id=NER.TemplateId
              INNER JOIN AppData.MiscMaster NotificationChannel on NotificationChannel.Id=NER.NotificationChannelId
              INNER JOIN AppData.MiscMaster RecipientType on RecipientType.Id=NER.RecipientTypeId
              WHERE NER.IsDeleted = 0
               AND (
              @Search IS NULL OR 
              NotificationChannel.Code LIKE '%' + @Search + '%' OR 
              RecipientType.Code LIKE '%' + @Search + '%'
                );
          ";

          var parameters = new
          {
              Search = string.IsNullOrEmpty(searchTerm) ? null : $"%{searchTerm}%",
              Offset = (pageNumber - 1) * pageSize,
              PageSize = pageSize
          };

          var result = await _dbConnection.QueryAsync<NotificationEventRule,Domain.Entities.Notification.MiscMaster, Domain.Entities.Notification.MiscMaster, NotificationEventRule>(
              dataQuery,
              (eventRule, notificationChannel,recipientType) =>
              {
                  eventRule.Channel = notificationChannel;
                  eventRule.RecipientType = recipientType;
                  return eventRule;
              },
              param: parameters,
              splitOn: "Id,Id"
          );

          var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

          return (result.ToList(), totalCount);
        }

        public async Task<bool> NotFoundAsync(int id)
        {
             var query = "SELECT COUNT(1) FROM [AppNotification].[NotificationEventRule] WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
        }
    }
}
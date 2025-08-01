using System.Data;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Queries.GetAllNotificationEventRule;
using BackgroundService.Domain.Entities.Notification;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Notification.NotificationEventRules
{
    public class NotificationEventRuleQueryRepository : INotificationEventRuleQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        public NotificationEventRuleQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
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

        public async Task<(IEnumerable<dynamic>, int)> GetAllNotificationEventRuleAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var UnitId = _ipAddressService.GetUnitId();

            var parameters = new
            {
                UnitId,
                Search = string.IsNullOrEmpty(SearchTerm) ? null : $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };
            
            var query = @"
                SELECT  
                NER.Id, NER.NotificationChannelId, NER.TemplateId, NER.NotificationLevelHierarchyId,
                NER.RecipientTypeId, NER.IsActive, NER.CreatedBy,
                NER.CreatedDate, NER.CreatedByName, NER.ModifiedBy, NER.ModifiedDate, NER.ModifiedByName,
                NotificationChannel.Code AS ChannelName,
                RecipientType.Code AS RecipientType,
                NC.ModuleName
                FROM [AppNotification].[NotificationEventRule] NER
                INNER JOIN AppNotification.NotificationLevelHierarchy NH on NH.Id=NER.NotificationLevelHierarchyId
                INNER JOIN AppNotification.NotificationTemplate NT on NT.Id=NER.TemplateId
                INNER JOIN AppData.MiscMaster NotificationChannel on NotificationChannel.Id=NER.NotificationChannelId
                INNER JOIN AppData.MiscMaster RecipientType on RecipientType.Id=NER.RecipientTypeId
                INNER JOIN AppNotification.NotificationConfig NC on NC.Id=NH.NotificationConfigId
                WHERE NC.UnitId=@UnitId 
                AND NER.IsDeleted = 0
                AND (@Search IS NULL OR NC.ModuleName LIKE @Search)
                ORDER BY NER.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*) 
                FROM [AppNotification].[NotificationEventRule] NER
                INNER JOIN AppNotification.NotificationLevelHierarchy NH on NH.Id=NER.NotificationLevelHierarchyId
                INNER JOIN AppNotification.NotificationTemplate NT on NT.Id=NER.TemplateId
                INNER JOIN AppData.MiscMaster NotificationChannel on NotificationChannel.Id=NER.NotificationChannelId
                INNER JOIN AppData.MiscMaster RecipientType on RecipientType.Id=NER.RecipientTypeId
                INNER JOIN AppNotification.NotificationConfig NC on NC.Id=NH.NotificationConfigId
                WHERE NC.UnitId=@UnitId 
                AND NER.IsDeleted = 0
                AND (@Search IS NULL OR NC.ModuleName LIKE @Search);
                ";
         
            using var multi = await _dbConnection.QueryMultipleAsync(query, parameters);
            var data = await multi.ReadAsync<NotificationEventRuleDto>();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (data.ToList(), totalCount);
        }

        public async Task<bool> NotFoundAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM [AppNotification].[NotificationEventRule] WHERE Id = @Id AND IsDeleted = 0";
             
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
            return count > 0;
        }
    }
}
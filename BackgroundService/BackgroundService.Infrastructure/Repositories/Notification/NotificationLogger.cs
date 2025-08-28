using System.Data;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Domain.Entities.Notification;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace BackgroundService.Infrastructure.Repositories.Notification
{
    public class NotificationLogger : INotificationLogger
    {
        private readonly IDbConnection _dbConnection;      

        public NotificationLogger([FromKeyedServices("Notification")] IDbConnection dbConnection)
        {
             _dbConnection = dbConnection;
        }
        public async Task<int> LogAsync(NotificationEventLog log)
        {
            var sql = @"
                    INSERT INTO AppNotification.NotificationEventLog
                    (NotificationLevelRuleId, NotificationStatusId, ReadStatusId, ActionStatus, SendTo, ChannelId, MessageText, Timestamp,
                    IsActive, IsDeleted, CreatedBy, CreatedDate, CreatedByName, CreatedIP)
                    OUTPUT INSERTED.Id
                    VALUES
                    (@NotificationLevelRuleId, @NotificationStatusId, @ReadStatusId, @ActionStatus, @SendTo, @ChannelId, @MessageText, @Timestamp,
                    @IsActive, @IsDeleted, @CreatedBy, @CreatedDate, @CreatedByName, @CreatedIP);";

            var insertedId = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                log.NotificationLevelRuleId,log.NotificationStatusId,log.ReadStatusId,log.ActionStatus,log.SendTo,log.ChannelId,
                log.MessageText,log.Timestamp,log.IsActive,log.IsDeleted,log.CreatedBy,log.CreatedDate,log.CreatedByName,log.CreatedIP
            });

            return insertedId;
        }   
    }
}
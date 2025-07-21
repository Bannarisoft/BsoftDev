using System.Data;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Domain.Entities.Notification;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Repositories.Notification
{
    public class NotificationLogger : INotificationLogger
    {
        private readonly IDbConnection _dbConnection;      

        public NotificationLogger(IDbConnection dbConnection)
        {
             _dbConnection = dbConnection;
        }
        public async Task LogAsync(NotificationEventLog log)
            {
                var sql = @"
                    INSERT INTO AppNotification.NotificationEventLog
                    (NotificationLevelRuleId, NotificationStatusId, ActionStatus, ChannelId, MessageText, Timestamp,
                    IsActive, IsDeleted, CreatedBy, CreatedDate, CreatedByName, CreatedIP)
                    VALUES
                    (@NotificationLevelRuleId, @NotificationStatusId, @ActionStatus, @ChannelId, @MessageText, @Timestamp,
                    @IsActive, @IsDeleted, @CreatedBy, @CreatedDate, @CreatedByName, @CreatedIP);";

                await _dbConnection.ExecuteAsync(sql, log);
            }   
    }
}
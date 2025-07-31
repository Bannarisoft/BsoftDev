
using System.Data;
using BackgroundService.Application.DTO;
using BackgroundService.Application.Interfaces.Notification;
using Dapper;

namespace BackgroundService.Infrastructure.Services.Notification
{
    public class NotificationUserResolver : INotificationUserResolver
    {
        private readonly IDbConnection _dbConnection;

        public NotificationUserResolver(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<NotificationTargetDto>> GetNotificationTargetsAsync(int unitId, string moduleName, int eventTypeId)
        {

             var parameters = new DynamicParameters();
                parameters.Add("@UnitId", unitId);
                parameters.Add("@ModuleName", moduleName);
                parameters.Add("@EventType", eventTypeId);

            var result = (await _dbConnection.QueryAsync<NotificationTargetDto>(
                "WorkFlow_GetUserId", 
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            return result;
        }
    }

}

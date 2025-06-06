using Microsoft.AspNetCore.SignalR;

namespace Core.Application.Common.RealTimeNotificationHub
{
    public class WorkOrderScheduleHub : Hub
    {        
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
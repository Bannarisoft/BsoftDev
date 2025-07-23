
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace BackgroundService.Application.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
        public override Task OnConnectedAsync()
         {
             var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             Console.WriteLine($"User connected: {userId}");
             return base.OnConnectedAsync();
         }
    }
}
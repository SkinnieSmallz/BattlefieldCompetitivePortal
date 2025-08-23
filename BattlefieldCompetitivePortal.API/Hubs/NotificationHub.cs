using BattlefieldCompetitivePortal.Framework.Models;
using Microsoft.AspNetCore.SignalR;
using System.Web.Mvc;

namespace BattlefieldCompetitivePortal.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task JoinRoleGroup(string role)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
        }
    }

    // Notification service
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUser(int userId, string title, string message)
        {
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("ReceiveNotification", new { title, message });
        }

        public async Task SendNotificationToRole (UserRole role, string title, string message)
        {
            await _hubContext.Clients.Group($"Role_ {role}")
                .SendAsync("ReceiveNotification", new { title, message });
        }
    }
}

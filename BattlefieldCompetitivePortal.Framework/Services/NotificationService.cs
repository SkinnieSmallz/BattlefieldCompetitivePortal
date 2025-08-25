using BattlefieldCompetitivePortal.Framework.Data;
using BattlefieldCompetitivePortal.Framework.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Services
{
    public class NotificationService
    {
        public async Task SendNotificationToRole(UserRole role, string title, string message, int notificationType)
        {
            // 1. Get all users with this role
            var queryUsers = @"
            SELECT UserId 
            FROM auth.Users
            WHERE Role = @Role AND IsActive = 1;";

            var parameters = new[]
            {
            new SqlParameter("@Role", (int)role)
        };

            var dt = await DatabaseHelper.ExecuteQueryAsync(queryUsers, parameters);

            // 2. Insert notifications for each user
            foreach (DataRow row in dt.Rows)
            {
                int userId = Convert.ToInt32(row["UserId"]);

                var insertQuery = @"
                INSERT INTO notifications.Notifications (UserId, Title, Message, NotificationType, IsRead, CreatedDate)
                VALUES (@UserId, @Title, @Message, @NotificationType, 0, GETDATE());";

                var insertParams = new[]
                {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Title", title),
                new SqlParameter("@Message", message),
                new SqlParameter("@NotificationType", notificationType)
            };

                await DatabaseHelper.ExecuteNonQueryAsync(insertQuery, insertParams);
            }
        }

        public async Task SendNotificationToUser(int userId, string title, string message, int notificationType)
        {
            var query = @"
            INSERT INTO notifications.Notifications (UserId, Title, Message, NotificationType, IsRead, CreatedDate)
            VALUES (@UserId, @Title, @Message, @NotificationType, 0, GETDATE());";

            var parameters = new[]
            {
            new SqlParameter("@UserId", userId),
            new SqlParameter("@Title", title),
            new SqlParameter("@Message", message),
            new SqlParameter("@NotificationType", notificationType)
        };

            await DatabaseHelper.ExecuteNonQueryAsync(query, parameters);
        }
    }

}

using BattlefieldCompetitivePortal.Framework.Data;
using BattlefieldCompetitivePortal.Framework.Models;
using BattlefieldCompetitivePortal.Framework.Security;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Services
{
    public class UserService
    {
        public async Task<User> ValidateUser(string username, string password)
        {
            try
            {
                var query = @"
                    SELECT UserId, Username, Email, PasswordHash, Role, TeamId, PlayerRole, CreatedDate, IsActive
                    FROM Users
                    WHERE Username = @Username AND IsActive = 1";

                var parameters = new[] { new SqlParameter("@username", username) };
                var dt = await DatabaseHelper.ExecuteQueryAsync(query, parameters);

                if (dt.Rows.Count == 0)
                    return null;

                var row = dt.Rows[0];
                var storedHash = row["PasswordHash"].ToString();

                if (!AuthenticationHelper.VerifyPassword(username, storedHash))
                    return null;

                return MapUserFromDataRow(row);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Authyentication failed", ex);
            }
        }

        public async Task<User> GetUserById(int userId)
        {
            var query = @"
                SELECT u.*, t.TeamName
                FROM Users u
                LEFT JOIN Teams t ON u.TeamId = t.TeamId
                WHERE u.UserId = @UserId AND u.IsActive = 1";

            var parameters = new[] { new SqlParameter("@UserId", userId) };
            var dt = await DatabaseHelper.ExecuteQueryAsync(query, parameters);

            return dt.Rows.Count > 0 ? MapUserFromDataRow(dt.Rows[0]) : null;
        }

        public async Task<bool> CreateUser(User user)
        {
            user.PasswordHash = AuthenticationHelper.HashPassword(user.PasswordHash);

            var query = @"
                INSERT INTO Users (Username, Email, PasswordHash, Role, CreatedDate, IsActive)
                VALUES (@Username, @Email, @PasswordHash, @Role, GETDATE(), 1);
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@Role", (int)user.Role),
            };

            var UserId = await DatabaseHelper.ExecuteScalarAsync<int>(query, parameters);
            user.UserId = UserId;

            return UserId > 0;
        }

        private User MapUserFromDataRow(DataRow row)
        {
            return new User
            {
                UserId = row.Field<int>("UserId"),
                Username = row.Field<string>("Username"),
                Email = row.Field<string>("Email"),
                PasswordHash = row.Field<string>("PasswordHash"),
                Role = (UserRole)row.Field<int>("Role"),
                TeamId = row.Field<int>("TeamId"),
                PlayerRole = row.Field<int?>("PlayerRole") != null
                    ? (PlayerRole)row.Field<int>("PlayerRole")
                    : null,
                CreatedDate = row.Field<DateTime>("CreatedDate"),
                IsActive = row.Field<bool>("IsActive")
            };
        }
    }
}

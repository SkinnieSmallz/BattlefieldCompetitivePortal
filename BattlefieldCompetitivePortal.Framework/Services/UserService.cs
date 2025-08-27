using BattlefieldCompetitivePortal.Framework.Data;
using BattlefieldCompetitivePortal.Framework.Models;
using BattlefieldCompetitivePortal.Framework.Security;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Services
{
    // Fixed UserService in Framework
    public class UserService
    {
        // Fixed: Added return type and corrected parameter name
        public async Task<User> ValidateUser(string username, string password)
        {
            try
            {
                var query = @"
                SELECT UserId, Username, Email, PasswordHash, Role, TeamId, PlayerRole, CreatedDate, IsActive
                FROM Users
                WHERE Username = @Username AND IsActive = 1";
                // Fixed: Parameter name should match the SQL parameter
                var parameters = new[] { new SqlParameter("@Username", username) };
                var dt = await DatabaseHelper.ExecuteQueryAsync(query, parameters);

                if (dt.Rows.Count == 0)
                    return null;

                var row = dt.Rows[0];
                var storedHash = row["PasswordHash"].ToString();

                // Fixed: Pass password instead of username to verify method
                if (!AuthenticationHelper.VerifyPassword(password, storedHash))
                    return null;

                return MapUserFromDataRow(row);
            }
            catch (Exception ex)
            {
                // Fixed: Spelling error
                throw new ApplicationException("Authentication failed", ex);
            }
        }

        // Fixed: Added return type
        public async Task<User> GetUserById(int userId)
        {
            var query = @"
            SELECT u.UserId, u.Username, u.Email, u.PasswordHash, u.Role, u.TeamId, 
                   u.PlayerRole, u.CreatedDate, u.IsActive, t.TeamName
            FROM Users u
            LEFT JOIN Teams t ON u.TeamId = t.TeamId
            WHERE u.UserId = @UserId AND u.IsActive = 1";

            var parameters = new[] { new SqlParameter("@UserId", userId) };
            var dt = await DatabaseHelper.ExecuteQueryAsync(query, parameters);  //CommandType.StoredProcedure

            return dt.Rows.Count > 0 ? MapUserFromDataRow(dt.Rows[0]) : null;
        }

        // Fixed: Added return type

        public async Task<bool> CreateUser(User user)
        {
            user.PasswordHash = AuthenticationHelper.HashPassword(user.PasswordHash);
            var storedProcedure = "auth.spUsers_Create";

            var parameters = new[]
            {
        new SqlParameter("@Username", string.IsNullOrEmpty(user.Username) ? (object)DBNull.Value : user.Username),
        new SqlParameter("@Email", string.IsNullOrEmpty(user.Email) ? (object)DBNull.Value : user.Email),
        new SqlParameter("@Name", string.IsNullOrEmpty(user.Name) ? (object)DBNull.Value : user.Name),
        new SqlParameter("@Surname", string.IsNullOrEmpty(user.Surname) ? (object)DBNull.Value : user.Surname),
        new SqlParameter("@ContactNumber", string.IsNullOrEmpty(user.ContactNumber) ? (object)DBNull.Value : user.ContactNumber),
        new SqlParameter("@PasswordHash", string.IsNullOrEmpty(user.PasswordHash) ? (object)DBNull.Value : user.PasswordHash),
        new SqlParameter("@Role", (int)user.Role),
        new SqlParameter("@TeamId", user.TeamId.HasValue ? (object)user.TeamId.Value : DBNull.Value),
        new SqlParameter("@PlayerRole", string.IsNullOrEmpty(user.PlayerRole.ToString()) ? (object)DBNull.Value : user.PlayerRole)
    };

            // Make sure you're only calling ExecuteScalarAsync ONCE
            var userId = await DatabaseHelper.ExecuteScalarAsync<int>(storedProcedure, parameters, CommandType.StoredProcedure);
            user.UserId = userId;
            return userId > 0;
        }
        //public async Task<bool> CreateUser(User user)
        //{
        //    user.PasswordHash = AuthenticationHelper.HashPassword(user.PasswordHash);

        //    var storedProcedure = "auth.spUsers_Create";

        //    // Define the parameters to pass to the stored procedure.
        //    var parameters = new[]
        //    {
        //    new SqlParameter("@Username", user.Username),
        //    new SqlParameter("@Email", user.Email),
        //    new SqlParameter("@Name", user.Name),
        //    new SqlParameter("@Surname", user.Surname),
        //    new SqlParameter("@ContactNumber", user.ContactNumber),
        //    new SqlParameter("@PasswordHash", user.PasswordHash),
        //    new SqlParameter("@Role", (int)user.Role),

        //    // Handle nullable parameters correctly.
        //    // If the value is null, send DBNull.Value to the database.
        //    new SqlParameter("@TeamId", (object)user.TeamId ?? DBNull.Value),
        //    new SqlParameter("@PlayerRole", (object)user.PlayerRole ?? DBNull.Value)
        //};

        //    // DEBUG: Check parameter values
        //    foreach (var param in parameters)
        //    {
        //        Debug.WriteLine($"{param.ParameterName}: '{param.Value}'");
        //    }

        //    // Execute the stored procedure and get the new user's ID back.
        //    // We use ExecuteScalarAsync because the stored procedure returns a single value (the new ID).
        //    var userId = await DatabaseHelper.ExecuteScalarAsync<int>(storedProcedure, parameters);

        //    user.UserId = userId;

        //    return userId > 0;

            //var query = @"
            //INSERT INTO Users (Username, Email, PasswordHash, Role, CreatedDate, IsActive)
            //VALUES (@Username, @Email, @PasswordHash, @Role, GETDATE(), 1);
            //SELECT SCOPE_IDENTITY();";

            //var parameters = new[]
            //{
            //new SqlParameter("@Username", user.Username),
            //new SqlParameter("@Email", user.Email),
            //new SqlParameter("@PasswordHash", user.PasswordHash),
            //new SqlParameter("@Role", (int)user.Role),
            //};

            //var userId = await DatabaseHelper.ExecuteScalarAsync<int>(query, parameters);
            //user.UserId = userId;

            //return userId > 0;
        //}

        private User MapUserFromDataRow(DataRow row)
        {
            return new User
            {
                UserId = row.Field<int>("UserId"),
                Username = row.Field<string>("Username"),
                Email = row.Field<string>("Email"),
                Name = row.Field<string>("Name"),
                Surname = row.Field<string>("Surname"),
                ContactNumber = row.Field<string>("ContactNumber"),
                PasswordHash = row.Field<string>("PasswordHash"),
                Role = (UserRole)row.Field<int>("Role"),
                TeamId = row.Field<int?>("TeamId"),
                PlayerRole = row.Field<int?>("PlayerRole") != null
                    ? (PlayerRole)row.Field<int>("PlayerRole")
                    : null,
                CreatedDate = row.Field<DateTime>("CreatedDate"),
                IsActive = row.Field<bool>("IsActive")
            };
        }
    }
}

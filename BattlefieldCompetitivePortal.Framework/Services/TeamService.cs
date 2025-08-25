using BattlefieldCompetitivePortal.Framework.Data;
using BattlefieldCompetitivePortal.Framework.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BattlefieldCompetitivePortal.Framework.Services
{
    public class TeamService
    {
        private readonly NotificationService notificationService;

        public TeamService(NotificationService notificationService = null)
        {
            this.notificationService = notificationService;
        }

        public TeamService CreateTeam(string teamName, int captainId)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var query = @"
                        INSERT INTO Teams (TeamName, CaptainId, CreatedDate, IsActive)
                        VALUES (@TeamName, @CaptainId, GETDATE(), 1);
                        SELECT SCOPE_IDENTITY();";

                    var parameters = new[]
                    {
                        new SqlParameter("@TeamName", teamName),
                        new SqlParameter("@CaptainId", captainId)
                    };

                    var teamId = DatabaseHelper.ExecuteScalarAsync<int>(query, parameters);

                    // Update captains team assignment
                    var updateCaptainQuery = @"
                        UPDATE Users
                        SET TeamId = @TeamId, PlayerRole = @PlayerRole
                        WHERE UserId = @CaptainId";

                    var updateParams = new[]
                    {
                        new SqlParameter("@TeamId", teamId),
                        new SqlParameter("@PlayerRole", (int)PlayerRole.Starter),
                        new SqlParameter("@CaptainId", captainId)
                    };

                    DatabaseHelper.ExecuteNonQueryAsync(updateCaptainQuery, updateParams);


                    transaction.Complete();

                    return new Team
                    {
                        TeamId = teamId,
                        TeamName = teamName,
                        CaptainId = captainId,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to create team", ex);
                }
            }
        }

        public async Task<bool> RequestToJoinTeam(int playerId, int teamId)
        {
            try
            {
                var query = @"
                    INSERT INTO TeamJoinRequests (PlayerId, TeamId, RequestDate, Status)
                    VALUES (@PlayerId, @TeamId, GETDATE(), 1)";

                var parameters = new[]
                {
                    new SqlParameter("@PlayerId", playerId),
                    new SqlParameter("@TeamId", teamId)
                };

                var result = await DatabaseHelper.ExecuteNonQueryAsync(query, parameters);

                if (result > 0 && _notificationService != null)
                {
                    // Notify team captain
                    var captain = GetTeamCaptain(teamId);
                    if (captain != null)
                    {
                        _notificationService.SendNotificationToUser(
                            captain.UserId,
                            "New Team Join Request",
                            "A player has requested to join your team"
                            );
                    }
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create join request", ex);
            }
        }

        public async Task<bool> ApproveJoinRequest(int requestId, int captainId)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    // Get request details
                    var requestQuery = @"
                        SELECT tjr.PlayerId, tjr.TeamId, u.Username
                        FROM TeamJoinRequests tjr
                        INNER JOIN Users u ON tjr.PlayerId = u.UserId
                        INNER JOIN Teams t on ON tjr.TeamId = t.TeamId
                        WHERE tjr.RequestId = @RequestId AND t.CaptainId = @CaptainId";

                    var requestParams = new[]
                    {
                        new SqlParameter("@RequestId", requestId),
                        new SqlParameter("@CaptainId", captainId)
                    };

                    var dt = await DatabaseHelper.ExecuteQueryAsync(requestQuery, requestParams);

                    if (dt.Rows.Count == 0)
                        throw new UnauthorizedAccessException("Invalid request or insufficient permissions");

                    var playerId = Convert.ToInt32(dt.Rows[0]["PlayerId"]);
                    var teamId = Convert.ToInt32(dt.Rows[0]["TeamId"]);
                    var playerName = dt.Rows[0]["Username"].ToString();

                    // Update player's team
                    var updatePlayerQuery = @"
                        UPDATE Users
                        SET TeamId = @TeamId, PlayerRole = @PlayerRole
                        WHERE UserId = playerId";

                    var updateParams = new[]
                    {
                        new SqlParameter("@TeamId", teamId),
                        new SqlParameter("@PlayerRole",(int)PlayerRole.Sub),
                        new SqlParameter("@PlayerId", playerId),
                    };

                    DatabaseHelper.ExecuteQueryAsync(requestQuery, requestParams);

                    // Update request status
                    var approveQuery = @"
                        UPDATE TeamJoinRequests
                        SET Status = 2
                        WHERE RequestId = @RequestId";

                    DatabaseHelper.ExecuteNonQueryAsync(approveQuery,
                        new[] { new SqlParameter("@RequestId", requestId) });

                    transaction.Complete();

                    // Send notifications to player
                    if (_notificationService != null)
                    {
                        _notificationService.SendNotificationToUser(
                            playerId,
                            "Team Join Request Approved",
                            $"You have been accepted to join the team!"
                        );
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to approve the join request", ex);
                }
            }
        }

        private async Task<User> GetTeamCaptainId(int teamId)
        {
            var query = @"
                SELECT u.* FROM Users u
                INNER JOIN Teams t ON u.UserId = t.CaptainId
                WHERE t.TeamId = @TeamId";

            var dt = await DatabaseHelper.ExecuteQueryAsync(query,
                new[] { new SqlParameter("@TeamId", teamId) });

            return dt.Rows.Count > 0 ? MapUserFromDataRow(dt.Rows[0]) : null;
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
                TeamId = row.Field<int?>("TeamId"),
                PlayerRole = row.Field<int?>("PlayerRole") is int pr ? (PlayerRole)pr : (PlayerRole?) null,
                CreatedDate = row.Field<DateTime>("CreatedDate"),
                IsActive = row.Field<bool>("IsActive")
            };
        }
    }
}

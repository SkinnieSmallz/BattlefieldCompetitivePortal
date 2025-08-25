using BattlefieldCompetitivePortal.Framework.Data;
using BattlefieldCompetitivePortal.Framework.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
//using BattlefieldCompetitivePortal.

namespace BattlefieldCompetitivePortal.Framework.Services
{
    public class ScrimService
    {
        private readonly NotificationService _notificationService;

        public ScrimService(NotificationService notificationService = null)
        {
            _notificationService = notificationService;
        }

        public async Task<List<Scrim>> GetUpcomingScrims()
        {
            var query = @"
                SELECT s.*, t1.TeamName as Team1Name, t2.TeamName as Team2Name,
                    u.Username as RequestedByName
                FROM Scrims s
                INNER JOIN Teams t1 ON s.Team1Id = t1.TeamId
                LEFT JOIN Teams t2 ON s.Team2Id = t2.TeamId
                INNER JOIN Users u ON s.RequestedBy = u.UserId
                WHERE s.ScheduledDate > GETDATE()
                AND s.Status IN (1,2) -- Pending or Approved
                ORDER BY s.ScheduledDate";

            var dt = await DatabaseHelper.ExecuteQueryAsync(query);
            return MapScrimsFromDataTable(dt);
        }

        public async Task<Scrim> CreateScrim(Scrim scrim)
        {
            var query = @"
                INSERT INTO SCRIMS (Team1Id, ScheduledDate, Status, RequestedBy, CreatedDate)
                VALUES (@Team1Id, @ScheduledDate, @Status, @RequestedBy, GETDATE());
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                new SqlParameter("@Team1Id", scrim.Team1Id),
                new SqlParameter("@ScheduledDate", scrim.ScheduledDate),
                new SqlParameter("@Status", (int)scrim.Status),
                new SqlParameter("@RequestedBy", scrim.RequestedBy)
            };

            var scrimId = await DatabaseHelper.ExecuteScalarAsync<int>(query, parameters);
            scrim.ScrimId = scrimId;

            return scrim;
        }

        public async Task<bool> RequestToJoinScrim(int scrimId, int teamId)
        {
            // Check if scrim exists and is available
            var checkQuery = @"
                SELECT Team1Id, Team2Id, Status
                FROM Scrims
                WHERE ScrimId = @ScrimId";

            var dt = await DatabaseHelper.ExecuteQueryAsync(checkQuery,
                new[] { new SqlParameter("@ScrimId", scrimId) });

            if (dt.Rows.Count == 0)
                throw new ArgumentException("Scrim not found");

            var row = dt.Rows[0];
            var team1Id = Convert.ToInt32(row["Team1Id"]);
            var team2Id = row["Team2Id"] as int?;
            var status = (ScrimStatus)Convert.ToInt32(row["status"]);

            if (team1Id == teamId)
                throw new InvalidOperationException("Cannot join your own scrim");

            if (team2Id.HasValue)
                throw new InvalidOperationException("Scrim is already full");

            if (status != ScrimStatus.Pending)
                throw new InvalidOperationException("Scrim is no longer available");

            // Update scrim with requesting team
            var updateQuery = @"
                UPDATE Scrims
                SET Team2Id = @TeamId
                WHERE ScrimId = @ScrimId";

            var parameters = new[]
            {
                new SqlParameter("@TeamId", teamId),
                new SqlParameter("@ScrimId", scrimId)
            };

            var result = await DatabaseHelper.ExecuteNonQueryAsync(updateQuery, parameters);

            if (result > 0 && _notificationService != null)
            {
                _notificationService.SendNotificationToRole(
                    UserRole.Admin,
                    "Scrim Join Request",
                    $"A team has requested to join a scrim #{scrimId}"
                );
            }

            return result > 0;
        }

        public async Task ApproveScrim(int scrimId, int? team2Id = null)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var updateQuery = @"
                        UPDATE Scrims 
                        SET Status = @Status" +
                        (team2Id.HasValue ? ", Team2Id = @Team2Id" : "") + @"
                        WHERE ScrimId = @ScrimId";

                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Status", (int)ScrimStatus.Approved),
                        new SqlParameter("@ScrimId", scrimId)
                    };

                    if (team2Id.HasValue)
                        parameters.Add(new SqlParameter("@Team2Id", team2Id.Value));

                    DatabaseHelper.ExecuteNonQueryAsync(updateQuery, parameters.ToArray());

                    // Get team captains for notifications
                    var teamsQuery = @"
                        SELECT s.Team1Id, s.Team2Id, t1.CaptainId as captain1Id, t2.CaptainId as Captain2Id
                        FROM Scrims s
                        INNER JOIN Teams t1 ON s.Team1Id = t1.Team1Id
                        LEFT JOIN Teams t2 ON s.Team2Id = t2.TeamId
                        WHERE s.ScrimId = @ScrimId";

                    var dt = await DatabaseHelper.ExecuteQueryAsync(teamsQuery,
                        new[] { new SqlParameter("@ScrimId", scrimId) });

                    if (dt.Rows.Count > 0 && _notificationService != null)
                    {
                        var row = dt.Rows[0];
                        var captain1Id = Convert.ToInt32(row["Captain1Id"]);
                        var captain2Id = row["Captain2Id"] as int?;

                        // Notify team captains
                        _notificationService.SendNotificationToUser(
                            captain1Id,
                            "Scrim Approved",
                            "Your scrim request has been approved!"
                        );

                        if (captain2Id.HasValue)
                        {
                            _notificationService.SendNotificationToUser(
                            captain2Id,
                            "Scrim Approved",
                            "Your scrim request has been approved!"
                            );
                        }
                    }

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to approve scrim", ex);
                }
            }
        }

        private List<Scrim> MapScrimsFromDataTable(DataTable dt)
        {
            return dt.AsEnumerable()
                .Select(row => new Scrim
                {
                    ScrimId = row.Field<int>("ScrimId"),
                    Team1Id = row.Field<int>("Team1Id"),
                    Team2Id = row.Field<int?>("Team2Id"),
                    ScheduledDate = row.Field<DateTime>("ScheduledDate"),
                    Status = (ScrimStatus)row.Field<int>("Status"),
                    WinnerTeamId = row.Field<int>("WinnerTeamId"),
                    RequestedBy = row.Field<int>("RequestedBy"),
                    CreatedDate = row.Field<DateTime>("CreatedDate"),
                    Team1Name = row.Field<string>("Team1Name"),
                    Team2Name = row.Field<string>("Team2Name"),
                    RequestedByName = row.Field<string>("RequestedByName")
                })

                .ToList();

        }
    }
}
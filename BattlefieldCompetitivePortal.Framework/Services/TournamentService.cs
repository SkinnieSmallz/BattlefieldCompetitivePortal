using BattlefieldCompetitivePortal.Framework.Data;
using BattlefieldCompetitivePortal.Framework.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Services
{
    public class TournamentService
    {
        public async Task GetAllTournaments()
        {

        }

        public async Task<Tournament> CreateTournament(Tournament tournament)
        {
            // Validate the tournament object
            if (tournament == null)
                throw new ArgumentNullException(nameof(tournament));

            // Set default values
            tournament.Status = TournamentStatus.Pending; // or whatever default status you want
            tournament.CreatedDate = DateTime.UtcNow;
            tournament.IsActive = true;

            try
            {
                // Build the INSERT query manually using your existing SafeQueryBuilder pattern
                var queryBuilder = new SafeQueryBuilder();

                // Build INSERT query string
                var insertQuery = @"INSERT INTO Tournaments 
            (TournamentName, Description, StartDate, EndDate, Status, CreatedBy, CreatedDate, IsActive) 
            VALUES 
            (@TournamentName, @Description, @StartDate, @EndDate, @Status, @CreatedBy, @CreatedDate, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() as int);";

                // Create parameters
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@TournamentName", tournament.TournamentName ?? (object)DBNull.Value),
            new SqlParameter("@Description", tournament.Description ?? (object)DBNull.Value),
            new SqlParameter("@StartDate", tournament.StartDate ?? (object)DBNull.Value),
            new SqlParameter("@EndDate", tournament.EndDate ?? (object)DBNull.Value),
            new SqlParameter("@Status", (int)tournament.Status),
            new SqlParameter("@CreatedBy", tournament.CreatedBy),
            new SqlParameter("@CreatedDate", tournament.CreatedDate),
            new SqlParameter("@IsActive", tournament.IsActive)
        };

                // Execute the query and get the new tournament ID
                var newTournamentId = await DatabaseHelper.ExecuteScalarAsync<int>(insertQuery, parameters.ToArray());

                // Set the ID on the tournament object
                tournament.TournamentId = newTournamentId;

                return tournament;
            }
            catch (Exception ex)
            {
                // Log the exception (assuming you have a logger)
                // _logger.LogError(ex, "Error creating tournament: {TournamentName}", tournament.TournamentName);
                throw new InvalidOperationException("Failed to create tournament", ex);
            }
        }
    }
}

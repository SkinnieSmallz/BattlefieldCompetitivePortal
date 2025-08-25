using BattlefieldCompetitivePortal.Framework.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Data
{
    public class SafeQueryBuilder
    {
        private List<SqlParameter> _parameters = new List<SqlParameter>();
        private readonly StringBuilder _query = new StringBuilder();

        public SafeQueryBuilder Select(string columns)
        {
            _query.Append($"SELECT{columns}");
            return this;
        }

        public SafeQueryBuilder From(string table)
        {
            _query.Append($"FROM{table}");
            return this;
        }

        public SafeQueryBuilder Where(string condition, string paramName, object value)
        {
            if (_query.ToString().Contains("WHERE"))
                _query.Append($"AND {condition}");
            else
                _query.Append($"WHERE{condition}");

            _parameters.Add(new SqlParameter($"@{paramName}", value ?? DBNull.Value));
            return this;
        }

        public async Task<DataTable> Execute()
        {
            return await DatabaseHelper.ExecuteQueryAsync(_query.ToString(), _parameters.ToArray());
        }

        public async Task<T> ExecuteScalar<T>()
        {
            return await DatabaseHelper.ExecuteScalarAsync<T>(_query.ToString(), _parameters.ToArray());
        }

        private List<Tournament> MapTournamentsFromDataTable(DataTable dataTable)
        {
            var tournaments = new List<Tournament>();

            if (dataTable == null || dataTable.Rows.Count == 0)
                return tournaments;

            foreach (DataRow row in dataTable.Rows)
            {
                var tournament = new Tournament
                {
                    TournamentId = Convert.ToInt32(row["TournamentId"]),
                    TournamentName = row["TournamentName"]?.ToString(),
                    Description = row["Description"]?.ToString(),
                    StartDate = (DateTime)(row["StartDate"] != DBNull.Value ? Convert.ToDateTime(row["StartDate"]) : (DateTime?)null),
                    EndDate = (DateTime)(row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null),
                    Status = (TournamentStatus)Convert.ToInt32(row["Status"])
                };

                tournaments.Add(tournament);
            }

            return tournaments;
        }

        public SafeQueryBuilder OrderBy(string column, bool ascending = true)
        {
            _query.Append($" ORDER BY {column}");
            if (!ascending)
                _query.Append(" DESC");
            return this;
        }

        public async Task<List<Tournament>> GetTournamentByStatus(TournamentStatus status, int? userId = null)
        {
            var queryBuilder = new SafeQueryBuilder()
                .Select(" TournamentId, TournamentName, Description, StartDate, EndDate, Status")
                .From(" Tournaments")
                .Where(" Status = @Status", "Status", (int)status)
                .Where(" IsActive = @IsActive", "IsActive", true);

            if (userId.HasValue)
            {
                queryBuilder.Where(" CreatedBy = @CreatedBy", "CreatedBy", userId.Value);
            }

            var dt = await queryBuilder.OrderBy("StartDate", true).Execute();
            return MapTournamentsFromDataTable(dt);
        }

        public SafeQueryBuilder Insert(string table)
        {
            _query.Clear();
            _parameters.Clear();
            _query.Append($"INSERT INTO {table}");
            return this;
        }

        public SafeQueryBuilder Values(Dictionary<string, object> columnValues)
        {
            var columns = string.Join(", ", columnValues.Keys);
            var paramNames = columnValues.Keys.Select(k => $"@{k}");
            var parameters = string.Join(", ", paramNames);

            _query.Append($" ({columns}) VALUES ({parameters}); SELECT CAST(SCOPE_IDENTITY() as int);");

            foreach (var kvp in columnValues)
            {
                _parameters.Add(new SqlParameter($"@{kvp.Key}", kvp.Value ?? DBNull.Value));
            }

            return this;
        }
    }
}



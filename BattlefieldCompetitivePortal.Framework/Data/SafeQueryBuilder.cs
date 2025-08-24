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

        public async DataTable Execute()
        {
            return await DatabaseHelper.ExecuteQueryAsync(_query.ToString(), _parameters.ToArray());
        }

        public async T ExecuteScalar<T>()
        {
            return await DatabaseHelper.ExecuteScalarAsync<T>(_query.ToString(), _parameters.ToArray());
        }

        public List<Tournament> GetTournamentByStatus(TournamentStatus status, int? userId = null)
        {
            var queryBuilder = new SafeQueryBuilder()
                .Select("TournamentId, TournamentName, Description, StartDate, EndDate, Status")
                .From("Tournaments")
                .Where("Status = @Status", "Status", (int)status)
                .Where("IsActive = @IsActive", "IsActive", true);

            if (userId.HasValue)
            {
                queryBuilder.Where("CreatedBy = @CreatedBy", "CreatedBy", userId.Value);
            }

            var dt = queryBuilder.OrderBy("Startdate", true).Execute();
            return MapTournamentsFromDataTable(dt);
        }

        // Add MapTournamentsFromDataTable method
    }
}



using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldCompetitivePortal.Framework.Data
{
    public static class DatabaseHelper
    {
        private static string ConnectionString =>
            ConfigurationManager.ConnectionStrings["BattlefieldPortal"].ConnectionString;

        public static async Task<DataTable> ExecuteQueryAsync(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    else
                    {
                        //error logging // throw here 
                    }

                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public static async Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                else
                {
                    //error logging // throw here 
                }

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();

            }
        }

        public static async Task<T> ExecuteScalarAsync<T>(string query, params SqlParameter[] parameters) //  Add try with block at last application layer 
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("Query cannot be null or empty", nameof(query));

            try
            {
                using var conn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(query, conn);

                if (parameters?.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    return default(T);

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                // Log exception
                throw; // or handle appropriately
            }
        }
    }
}
    





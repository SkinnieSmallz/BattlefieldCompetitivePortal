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
using System.Web;
//using BattlefieldCompetitivePortal.MVC;

namespace BattlefieldCompetitivePortal.Framework.Data
{
    public static class DatabaseHelper
    {
        public static string ConnectionString { get; set; } = string.Empty;
    //ConfigurationManager.ConnectionStrings["BattlefieldPortal"]?.ConnectionString
    //?? throw new InvalidOperationException("Connection string 'BattlefieldPortal' not found in configuration");

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
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null, empty, or whitespace.", nameof(query));

            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<T> ExecuteScalarAsync<T>(
            string query,
            SqlParameter[] parameters = null,
            CommandType commandType = CommandType.Text) //  Add try with block at last application layer 
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty", nameof(query));

            try
            {
                using var conn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand(query, conn);

                cmd.CommandType = commandType;

                if (parameters?.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    return default;

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch
            {
                // TODO: log exception
                throw;
            }
            //updated //if (string.IsNullOrWhiteSpace(query))
            //    throw new ArgumentException("Query cannot be null or empty", nameof(query));

            //try
            //{
            //    using var conn = new SqlConnection(ConnectionString);
            //    using var cmd = new SqlCommand(query, conn);

            //    // Set the command type (Text, StoredProcedure, etc.)
            //    cmd.CommandType = commandType;

            //    if (parameters?.Length > 0)
            //        cmd.Parameters.AddRange(parameters);

            //    await conn.OpenAsync();
            //    var result = await cmd.ExecuteScalarAsync();

            //    if (result == null || result == DBNull.Value)
            //        return default;

            //    return (T)Convert.ChangeType(result, typeof(T));
            //}
            //catch (Exception ex)
            //{
            //    // It's a good practice to log the exception here.
            //    // For now, we re-throw it to be handled by the calling service.
            //    throw;
            //}



            //old //if (string.IsNullOrWhiteSpace(query))
            //    throw new ArgumentException("Query cannot be null or empty", nameof(query));

            //try
            //{
            //    using var conn = new SqlConnection(ConnectionString);
            //    using var cmd = new SqlCommand(query, conn);

            //    if (parameters?.Length > 0)
            //        cmd.Parameters.AddRange(parameters);

            //    await conn.OpenAsync();
            //    var result = await cmd.ExecuteScalarAsync();

            //    if (result == null || result == DBNull.Value)
            //        return default;

            //    return (T)Convert.ChangeType(result, typeof(T));
            //}
            //catch (Exception ex)
            //{
            //    // Log exception
            //    throw; // or handle appropriately
            //}
        }
    }
}






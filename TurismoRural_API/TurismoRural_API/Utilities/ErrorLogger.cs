using Dapper;
using System.Data;
using TurismoRural_API.Repositories;

namespace TurismoRural_API.Utilities
{
    public static class ErrorLogger
    {
        public static async Task LogAsync(DapperContext context, string source, string message, string? stackTrace = null)
        {
            try
            {
                using var connection = context.CreateConnection();
                var sql = "INSERT INTO ErrorLogs (Source, Message, StackTrace, CreatedAt) VALUES (@Source, @Message, @StackTrace, @CreatedAt)";
                await connection.ExecuteAsync(sql, new { Source = source, Message = message, StackTrace = stackTrace, CreatedAt = DateTime.UtcNow });
            }
            catch
            {
                // Swallow exceptions to avoid cascading failures when logging
            }
        }
    }
}

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
                await connection.ExecuteAsync(
                    "SP_RegistrarError",
                    new { MensajeError = message, StackTrace = stackTrace, Metodo = source },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                // Swallow exceptions to avoid cascading failures when logging
            }
        }
    }
}

using Dapper;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;

namespace TurismoRural_API.Repositories
{
    public class ExperienciaConcurrenciaRepository : IExperienciaConcurrenciaRepository
    {
        private readonly DapperContext _context;

        public ExperienciaConcurrenciaRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExperienciaConcurrencia>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<ExperienciaConcurrencia>(
                "SP_ConsultarExperienciasConcurrencia",
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<ExperienciaConcurrencia?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Concurrencia", id);

            return await connection.QueryFirstOrDefaultAsync<ExperienciaConcurrencia>(
                "SP_ConsultarExperienciaConcurrenciaPorId",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(ExperienciaConcurrenciaCreateRequest model)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Fecha", model.Fecha.Date);
            parameters.Add("@Detalle", model.Detalle);
            parameters.Add("@Precio", model.Precio);
            parameters.Add("@Cupos_Disponibles", model.Cupos_Disponibles);
            parameters.Add("@ID_Experiencia", model.ID_Experiencia);

            await connection.ExecuteAsync(
                "SP_InsertarExperienciaConcurrencia",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            var id = await connection.QuerySingleAsync<int>(
                @"SELECT TOP 1 ID_Concurrencia
                  FROM ExperienciaConcurrencia
                  WHERE Fecha = @Fecha
                    AND Cupos_Disponibles = @Cupos_Disponibles
                    AND ID_Experiencia = @ID_Experiencia
                  ORDER BY ID_Concurrencia DESC;",
                parameters);

            await connection.ExecuteAsync(
                "UPDATE ExperienciaConcurrencia SET Precio = @Precio WHERE ID_Concurrencia = @ID_Concurrencia;",
                new { ID_Concurrencia = id, model.Precio });

            return id;
        }

        public async Task<bool> UpdateAsync(int id, ExperienciaConcurrenciaUpdateRequest model)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Concurrencia", id);
            parameters.Add("@Fecha", model.Fecha?.Date);
            parameters.Add("@Detalle", model.Detalle);
            parameters.Add("@Precio", model.Precio);
            parameters.Add("@Cupos_Disponibles", model.Cupos_Disponibles);
            parameters.Add("@ID_Experiencia", model.ID_Experiencia);

            var affected = await connection.ExecuteAsync(
                "SP_ActualizarExperienciaConcurrencia",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            if (model.Precio.HasValue)
            {
                await connection.ExecuteAsync(
                    "UPDATE ExperienciaConcurrencia SET Precio = @Precio WHERE ID_Concurrencia = @ID_Concurrencia;",
                    new { ID_Concurrencia = id, Precio = model.Precio.Value });
            }

            var exists = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(1) FROM ExperienciaConcurrencia WHERE ID_Concurrencia = @ID_Concurrencia;",
                new { ID_Concurrencia = id });

            return affected > 0 || exists > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Concurrencia", id);

            var result = await connection.QuerySingleAsync<int>(
                "SP_EliminarExperienciaConcurrencia",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }
    }
}

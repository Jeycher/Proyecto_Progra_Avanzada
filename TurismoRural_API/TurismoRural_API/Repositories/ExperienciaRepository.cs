using Dapper;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;

namespace TurismoRural_API.Repositories
{
    public class ExperienciaRepository : IExperienciaRepository
    {
        private readonly DapperContext _context;

        public ExperienciaRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Experiencia>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Experiencia>(
                "SP_ConsultarExperiencias",
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Experiencia?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Experiencia", id);

            return await connection.QueryFirstOrDefaultAsync<Experiencia>(
                "SP_ConsultarExperienciasPorId",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(ExperienciaCreateRequest model)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Titulo", model.Titulo);
            parameters.Add("@Descripcion", model.Descripcion);
            parameters.Add("@Categoria", model.Categoria);
            parameters.Add("@UsuarioIdRegistrador", model.UsuarioIdRegistrador);
            parameters.Add("@ID_Comunidad", model.ID_Comunidad);

            await connection.ExecuteAsync(
                "SP_InsertarExperiencia",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return await connection.QuerySingleAsync<int>(
                @"SELECT TOP 1 ID_Experiencia
                  FROM Experiencia
                  WHERE Titulo = @Titulo
                    AND UsuarioIdRegistrador = @UsuarioIdRegistrador
                    AND ID_Comunidad = @ID_Comunidad
                  ORDER BY ID_Experiencia DESC;",
                parameters);
        }

        public async Task<bool> UpdateAsync(int id, ExperienciaUpdateRequest model)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Experiencia", id);
            parameters.Add("@Titulo", model.Titulo);
            parameters.Add("@Descripcion", model.Descripcion);
            parameters.Add("@Categoria", model.Categoria);
            parameters.Add("@UsuarioIdRegistrador", model.UsuarioIdRegistrador);
            parameters.Add("@ID_Comunidad", model.ID_Comunidad);

            var affected = await connection.ExecuteAsync(
                "SP_ActualizarExperiencia",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Experiencia", id);

            var affected = await connection.ExecuteAsync(
                "SP_EliminarExperiencia",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return affected > 0;
        }
    }
}

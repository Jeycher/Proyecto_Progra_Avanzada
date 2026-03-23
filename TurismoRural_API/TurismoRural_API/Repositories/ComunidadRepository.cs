using Dapper;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;

namespace TurismoRural_API.Repositories
{
    public class ComunidadRepository : IComunidadRepository
    {
        private readonly DapperContext _context;

        public ComunidadRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comunidad>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Comunidad>("SP_ConsultarComunidades", commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(Comunidad comunidad)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Nombre_Comunidad", comunidad.Nombre_Comunidad);
            parameters.Add("@Pais", comunidad.Pais);
            parameters.Add("@Provincia", comunidad.Provincia);
            parameters.Add("@Descripcion", comunidad.Descripcion);

            await connection.ExecuteAsync("SP_InsertarComunidad", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return await connection.QuerySingleAsync<int>(
                @"SELECT TOP 1 ID_Comunidad
                  FROM Comunidad
                  WHERE Nombre_Comunidad = @Nombre_Comunidad AND Pais = @Pais
                  ORDER BY ID_Comunidad DESC;", parameters);
        }

        public async Task<bool> UpdateAsync(Comunidad comunidad)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Comunidad", comunidad.ID_Comunidad);
            parameters.Add("@Nombre_Comunidad", comunidad.Nombre_Comunidad);
            parameters.Add("@Pais", comunidad.Pais);
            parameters.Add("@Provincia", comunidad.Provincia);
            parameters.Add("@Descripcion", comunidad.Descripcion);

            await connection.ExecuteAsync("SP_ActualizarComunidad", parameters, commandType: System.Data.CommandType.StoredProcedure);

            var exists = await connection.QuerySingleAsync<int>("SELECT COUNT(1) FROM Comunidad WHERE ID_Comunidad = @ID_Comunidad;", parameters);
            return exists > 0;
        }
    }
}

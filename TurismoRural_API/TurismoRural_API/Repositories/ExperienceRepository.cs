using Dapper;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;

namespace TurismoRural_API.Repositories
{
    public class ExperienceRepository : IExperienceRepository
    {
        private readonly DapperContext _context;

        public ExperienceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Experience experience)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Title", experience.Title);
            parameters.Add("@Description", experience.Description);
            parameters.Add("@Price", experience.Price);
            parameters.Add("@HostId", experience.HostId);
            parameters.Add("@CreatedAt", experience.CreatedAt);

            var id = await connection.QuerySingleAsync<int>("sp_CrearExperience", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            var affected = await connection.ExecuteAsync("sp_EliminarExperience", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return affected > 0;
        }

        public async Task<IEnumerable<Experience>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Experience>("sp_ObtenerExperiencias", commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Experience?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return await connection.QueryFirstOrDefaultAsync<Experience>("sp_ObtenerExperiencePorId", parameters, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateAsync(Experience experience)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", experience.Id);
            parameters.Add("@Title", experience.Title);
            parameters.Add("@Description", experience.Description);
            parameters.Add("@Price", experience.Price);
            parameters.Add("@HostId", experience.HostId);

            var affected = await connection.ExecuteAsync("sp_ActualizarExperience", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return affected > 0;
        }
    }
}
using Dapper;
using System.Data;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;

namespace TurismoRural_API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(User user)
        {
            // Uses stored procedure 'sp_RegistrarUsuario' which should return the new Id as an integer
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@FullName", user.Nombre);
            parameters.Add("@Email", user.Correo);
            parameters.Add("@PasswordHash", user.Contrasena);
            parameters.Add("@Role", user.ID_Rol);
            parameters.Add("@CreatedAt", user.Fecha_Registro);

            var id = await connection.QuerySingleAsync<int>("sp_RegistrarUsuario", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return id;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>("SP_ConsultarUsuarios", commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            return await connection.QueryFirstOrDefaultAsync<User>("sp_ObtenerUsuarioPorEmail", parameters, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return await connection.QueryFirstOrDefaultAsync<User>("sp_ObtenerUsuarioPorId", parameters, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
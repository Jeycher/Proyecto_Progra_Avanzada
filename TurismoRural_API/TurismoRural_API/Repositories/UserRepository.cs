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
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Nombre", user.FullName);
            parameters.Add("@Correo", user.Email);
            parameters.Add("@Telefono", (string?)null);
            parameters.Add("@Contrasena", user.PasswordHash ?? string.Empty);
            parameters.Add("@ID_Rol", string.Equals(user.Role, "Administrador", StringComparison.OrdinalIgnoreCase) ? 1 : 2);

            await connection.ExecuteAsync("SP_RegistrarUsuario", parameters, commandType: System.Data.CommandType.StoredProcedure);
            var id = await connection.QuerySingleAsync<int>(
                @"SELECT TOP 1 ID_Usuario
                  FROM Usuario
                  WHERE Correo = @Correo
                  ORDER BY ID_Usuario DESC;", parameters);
            return id;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>(
                @"SELECT
                    ID_Usuario AS Id,
                    Nombre AS FullName,
                    Correo AS Email,
                    Contrasena AS PasswordHash,
                    CASE WHEN ID_Rol = 1 THEN 'Administrador' ELSE 'User' END AS Role,
                    Fecha_Registro AS CreatedAt
                  FROM Usuario");
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Correo", email);
            return await connection.QueryFirstOrDefaultAsync<User>(
                @"SELECT
                    ID_Usuario AS Id,
                    Nombre AS FullName,
                    Correo AS Email,
                    Contrasena AS PasswordHash,
                    CASE WHEN ID_Rol = 1 THEN 'Administrador' ELSE 'User' END AS Role,
                    Fecha_Registro AS CreatedAt
                  FROM Usuario
                  WHERE Correo = @Correo", parameters);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Usuario", id);
            return await connection.QueryFirstOrDefaultAsync<User>(
                @"SELECT
                    ID_Usuario AS Id,
                    Nombre AS FullName,
                    Correo AS Email,
                    Contrasena AS PasswordHash,
                    CASE WHEN ID_Rol = 1 THEN 'Administrador' ELSE 'User' END AS Role,
                    Fecha_Registro AS CreatedAt
                  FROM Usuario
                  WHERE ID_Usuario = @ID_Usuario", parameters);
        }
    }
}
using Dapper;
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
            parameters.Add("@Nombre", user.Nombre);
            parameters.Add("@Correo", user.Correo);
            parameters.Add("@Telefono", user.Telefono);
            parameters.Add("@Contrasena", user.Contrasena ?? string.Empty);
            parameters.Add("@ID_Rol", user.ID_Rol);

            await connection.ExecuteAsync(
                "SP_RegistrarUsuario",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            var id = await connection.QuerySingleAsync<int>(
                @"SELECT TOP 1 ID_Usuario
                  FROM Usuario
                  WHERE Correo = @Correo
                  ORDER BY ID_Usuario DESC;",
                parameters);

            return id;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<User>(
                @"SELECT
                    ID_Usuario,
                    Nombre,
                    Correo,
                    Contrasena,
                    Telefono,
                    ID_Rol,
                    Fecha_Registro
                  FROM Usuario");
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Correo", email);

            return await connection.QueryFirstOrDefaultAsync<User>(
                @"SELECT
                    ID_Usuario,
                    Nombre,
                    Correo,
                    Contrasena,
                    Telefono,
                    ID_Rol,
                    Fecha_Registro
                  FROM Usuario
                  WHERE Correo = @Correo",
                parameters);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ID_Usuario", id);

            return await connection.QueryFirstOrDefaultAsync<User>(
                @"SELECT
                    ID_Usuario,
                    Nombre,
                    Correo,
                    Contrasena,
                    Telefono,
                    ID_Rol,
                    Fecha_Registro
                  FROM Usuario
                  WHERE ID_Usuario = @ID_Usuario",
                parameters);
        }

        public async Task<User?> LoginAsync(string correo, string contrasena)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Correo", correo);
            parameters.Add("@Contrasena", contrasena);

            return await connection.QueryFirstOrDefaultAsync<User>(
                "SP_LoginUsuario",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using var connection = _context.CreateConnection();

            var rows = await connection.ExecuteAsync(
                @"UPDATE Usuario
                  SET Nombre = @Nombre,
                      Correo = @Correo,
                      Telefono = @Telefono,
                      ID_Rol = @ID_Rol
                  WHERE ID_Usuario = @ID_Usuario",
                new
                {
                    user.ID_Usuario,
                    user.Nombre,
                    user.Correo,
                    user.Telefono,
                    user.ID_Rol
                });

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var rows = await connection.ExecuteAsync(
                @"DELETE FROM Usuario
                  WHERE ID_Usuario = @ID_Usuario",
                new { ID_Usuario = id });

            return rows > 0;
        }
    }
}



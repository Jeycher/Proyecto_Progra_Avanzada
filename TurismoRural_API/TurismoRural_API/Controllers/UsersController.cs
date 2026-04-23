using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;
using TurismoRural_API.Repositories;
using TurismoRural_API.Services;
using TurismoRural_API.Utilities;

namespace TurismoRural_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly DapperContext _context;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IConfiguration _config;

        public UsersController(IUserRepository userRepository, DapperContext context, IPasswordHelper passwordHelper, IConfiguration config)
        {
            _userRepository = userRepository;
            _context = context;
            _passwordHelper = passwordHelper;
            _config = config;
        }

        // ============================================================================
        // AUTHENTICATION ENDPOINTS
        // ============================================================================

        [AllowAnonymous]
        [HttpPost("RegistroUsuario")]
        public async Task<IActionResult> RegistroUsuario(RegistrarUsuarioRequest model)
        {
            using var context = new SqlConnection(_config.GetValue<string>("ConnectionStrings:DefaultConnection"));
            var parametros = new DynamicParameters();
            parametros.Add("@Nombre", model.Nombre);
            parametros.Add("@Correo", model.CorreoElectronico);
            parametros.Add("@Telefono", model.Telefono ?? string.Empty);
            parametros.Add("@Contrasena", _passwordHelper.Encrypt(model.Contrasenna));
            parametros.Add("@ID_Rol", 2); // Default role for new users

            try
            {
                var id = context.QueryFirstOrDefault<int>(
                    "SP_RegistrarUsuario", 
                    parametros, 
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (id <= 0)
                    return BadRequest("Su información no se registró correctamente");

                var response = new RegistroResponse
                {
                    Id = id,
                    Nombre = model.Nombre,
                    Correo = model.CorreoElectronico,
                    Mensaje = "Su información se registró correctamente"
                };

                return CreatedAtAction(nameof(GetById), new { id = id }, response);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(UsersController) + ".RegistroUsuario", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [AllowAnonymous]
        [HttpPost("IniciarSesion")]
        public IActionResult IniciarSesion(IniciarSesionRequest model)
        {
            using var context = new SqlConnection(_config.GetValue<string>("ConnectionStrings:DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@Correo", model.CorreoElectronico);

            var user = context.QueryFirstOrDefault<dynamic>(
                @"SELECT ID_Usuario, Nombre, Correo, Contrasena, ID_Rol
                  FROM Usuario
                  WHERE Correo = @Correo",
                parametros);

            if (user == null)
                return NotFound("Su información no se autenticó correctamente");

            var encryptedPassword = _passwordHelper.Encrypt(model.Contrasenna);
            if (encryptedPassword != user.Contrasena)
                return NotFound("Su información no se autenticó correctamente");

            var response = new UsuarioResponse
            {
                Id = user.ID_Usuario,
                Nombre = user.Nombre,
                Correo = user.Correo,
                Rol = user.ID_Rol,
                Token = GenerarToken(user.ID_Usuario)
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPut("RecuperarAcceso")]
        public IActionResult RecuperarAcceso(RecuperarAccesoRequest model)
        {
            using var context = new SqlConnection(_config.GetValue<string>("ConnectionStrings:DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@Correo", model.CorreoElectronico);

            var user = context.QueryFirstOrDefault<dynamic>(
                @"SELECT ID_Usuario, Nombre, Correo
                  FROM Usuario
                  WHERE Correo = @Correo",
                parametros);

            if (user == null)
                return NotFound("Su información no se validó correctamente");

            var nuevaContrasenna = GenerarContrasenna();

            var parametrosActualizacion = new DynamicParameters();
            parametrosActualizacion.Add("@ID_Usuario", user.ID_Usuario);
            parametrosActualizacion.Add("@Contrasena", _passwordHelper.Encrypt(nuevaContrasenna));

            var actualizacion = context.Execute(
                "UPDATE Usuario SET Contrasena = @Contrasena WHERE ID_Usuario = @ID_Usuario",
                parametrosActualizacion);

            if (actualizacion <= 0)
                return BadRequest("No se pudo recuperar el acceso");

            var contenido = ObtenerPlantillaCorreo(user.Nombre, nuevaContrasenna);
            _passwordHelper.EnviarCorreo(user.Correo, "Recuperación de Acceso", contenido);

            return Ok("Se ha enviado una nueva contraseña a su correo electrónico");
        }

        // ============================================================================
        // CRUD ENDPOINTS
        // ============================================================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(UsersController) + ".GetAll", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(UsersController) + ".GetById", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (user == null || id != user.ID_Usuario)
                return BadRequest();

            try
            {
                var updated = await _userRepository.UpdateAsync(user);

                if (!updated)
                    return NotFound();

                return Ok("Usuario actualizado correctamente.");
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(UsersController) + ".Update", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _userRepository.DeleteAsync(id);

                if (!deleted)
                    return NotFound();

                return Ok("Usuario eliminado correctamente.");
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(UsersController) + ".Delete", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // ============================================================================
        // PRIVATE HELPER METHODS
        // ============================================================================

        private static string GenerarContrasenna()
        {
            const string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string([.. Enumerable.Range(0, 8).Select(_ => letras[Random.Shared.Next(letras.Length)])]);
        }

        private static string ObtenerPlantillaCorreo(string nombre, string contrasenna)
        {
            return $@"
                <html>
                <body>
                    <h2>Hola {nombre},</h2>
                    <p>Se ha solicitado la recuperación de acceso a tu cuenta.</p>
                    <p>Tu nueva contraseña temporal es: <strong>{contrasenna}</strong></p>
                    <p>Te recomendamos cambiarla tan pronto ingreses a la plataforma.</p>
                    <p>Saludos,<br>El equipo de TurismoRural</p>
                </body>
                </html>";
        }

        private string GenerarToken(int consecutivo)
        {
            var key = Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:Key")!);

            var claims = new[]
            {
                new Claim("consecutivo", consecutivo.ToString()),
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var tokenDescriptor = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}

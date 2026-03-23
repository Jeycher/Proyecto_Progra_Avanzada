using Dapper;
using TurismoRural_API.Models;
using TurismoRural_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TurismoRural_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SeguridadController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHelper _password;
        public SeguridadController(IConfiguration config, IPasswordHelper password)
        {
            _config = config;
            _password = password;
        }

        [HttpPut("CambiarAcceso")]
        public IActionResult CambiarAcceso(SeguridadRequest model)
        {
            var consecutivo = User.FindFirst("consecutivo")?.Value;

            using var context = new SqlConnection(_config.GetValue<string>("ConnectionStrings:DefaultConnection"));
            var parametros = new DynamicParameters();
            parametros.Add("@Consecutivo", consecutivo);
            parametros.Add("@Contrasenna", model.NuevaContrasenna);

            var result = context.Execute("sp_ActualizarContrasenna", parametros);

            if (result <= 0)
                return BadRequest("Su información no se actualizó correctamente");

            return Ok("Su información se actualizó correctamente");
        }

        [HttpGet("ConsultarUsuario")]
        public IActionResult ConsultarUsuario()
        {
            var consecutivo = User.FindFirst("consecutivo")?.Value;

            using var context = new SqlConnection(_config.GetValue<string>("ConnectionStrings:DefaultConnection"));
            var parametros = new DynamicParameters();
            parametros.Add("@Consecutivo", consecutivo);

            var result = context.QueryFirstOrDefault<UsuarioResponse>("sp_ConsultarUsuario", parametros);

            if (result == null)
                return NotFound("Su información no se validó correctamente");

            return Ok(result);
        }

        [HttpPut("CambiarPerfil")]
        public IActionResult CambiarPerfil(PerfilRequest model)
        {
            var consecutivo = User.FindFirst("consecutivo")?.Value;

            using var context = new SqlConnection(_config.GetValue<string>("ConnectionStrings:DefaultConnection"));
            var parametros = new DynamicParameters();
            parametros.Add("@Consecutivo", consecutivo);
            parametros.Add("@Identificacion", model.Identificacion);
            parametros.Add("@Nombre", model.Nombre);
            parametros.Add("@CorreoElectronico", model.CorreoElectronico);
            parametros.Add("@ImagenPerfil", model.ImagenPerfil);

            var result = context.Execute("sp_ActualizarPerfil", parametros);

            if (result <= 0)
                return BadRequest("Su información no se actualizó correctamente");

            return Ok("Su información se actualizó correctamente");
        }
        
    }
}

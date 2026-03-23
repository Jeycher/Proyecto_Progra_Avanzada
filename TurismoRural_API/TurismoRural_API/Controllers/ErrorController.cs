using Dapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace TurismoRural_API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ErrorController(IConfiguration config)
        {
            _config = config;
        }

        [Route("CapturarError")]
        public IActionResult CapturarError()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var usuario = User.FindFirst("consecutivo")?.Value ?? "0";

            using var context = new SqlConnection(_config.GetValue<string>("ConnectionStrings:DefaultConnection"));
            var parametros = new DynamicParameters();
            parametros.Add("@Error", exception?.Error.Message);
            parametros.Add("@Fecha", DateTime.Now);
            parametros.Add("@Origen", exception?.Path);
            parametros.Add("@Usuario", usuario);

            try
            {
                context.Execute("sp_RegistrarError", parametros);
            }
            catch
            {
                // ignore logging errors
            }

            return StatusCode(500, "Ocurrió un error interno");
        }
    }
}

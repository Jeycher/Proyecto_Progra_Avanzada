using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using TurismoRural_WEB.Services;

namespace TurismoRural_WEB.Pages
{
    public class SesionesModel : PageModel
    {
        private readonly ExperienciasApiService _experienciasService;

        public SesionesModel(ExperienciasApiService experienciasService)
        {
            _experienciasService = experienciasService;
        }

        public string RolUsuario { get; set; }
        public bool EsAdmin { get; set; }
        public bool EsUsuario { get; set; }
        public bool EsLoggedIn { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            var rolUsuario = HttpContext.Session.GetString("RolUsuario");

            EsLoggedIn = userId != null && (rolUsuario == "1" || rolUsuario == "2");
            RolUsuario = rolUsuario ?? string.Empty;
            EsAdmin = rolUsuario == "1";
            EsUsuario = rolUsuario == "2";

            return Page();
        }

        // Obtener concurrencias filtradas por experienciaId
        public async Task<IActionResult> OnGetGetExperiencias([FromQuery] int experienciaId = 0)
        {
            try
            {
                var concurrencias = await _experienciasService.GetConcurrenciasAsync();

                if (experienciaId > 0)
                {
                    concurrencias = concurrencias.Where(c =>
                    {
                        if (c is JsonElement je &&
                            je.TryGetProperty("iD_Experiencia", out var prop) &&
                            prop.TryGetInt32(out int id))
                        {
                            return id == experienciaId;
                        }
                        return false;
                    }).ToList();
                }

                return new JsonResult(concurrencias);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }

        // Reservar una sesión (solo para usuarios normales)
        public async Task<IActionResult> OnPostReservar([FromBody] ReservarRequest request)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return new JsonResult(new { success = false, requiresLogin = true });

            var reserva = new
            {
                iD_Usuario = usuarioId,
                iD_Concurrencia = request.ConcurrenciaId,
                cantidad_Personas = request.CantidadPersonas,
                estado = 1
            };

            var result = await _experienciasService.CreateReservationAsync(reserva);
            return new JsonResult(new { success = result });
        }

        public class ReservarRequest
        {
            public int ConcurrenciaId { get; set; }
            public int CantidadPersonas { get; set; }
        }

        // Obtener experiencias para el select
        public async Task<IActionResult> OnGetGetExperienciasSelect()
        {
            var experiencias = await _experienciasService.GetExperienciasAsync();
            return new JsonResult(experiencias);
        }

        // Crear nueva concurrencia
        public async Task<IActionResult> OnPostCreate([FromBody] dynamic concurrencia)
        {
            var result = await _experienciasService.CreateConcurrenciaAsync(concurrencia);
            return new JsonResult(new { success = result });
        }

        // Actualizar concurrencia
        public async Task<IActionResult> OnPostUpdate(int id, [FromBody] dynamic concurrencia)
        {
            var result = await _experienciasService.UpdateConcurrenciaAsync(id, concurrencia);
            return new JsonResult(new { success = result });
        }

        // Eliminar concurrencia
        public async Task<IActionResult> OnPostDelete(int id)
        {
            Console.Error.WriteLine($"OnPostDelete called with id={id}");
            try
            {
                var result = await _experienciasService.DeleteConcurrenciaAsync(id);
                Console.Error.WriteLine($"OnPostDelete result={result}");
                return new JsonResult(new { success = result });
            }
            catch (InvalidOperationException ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
}

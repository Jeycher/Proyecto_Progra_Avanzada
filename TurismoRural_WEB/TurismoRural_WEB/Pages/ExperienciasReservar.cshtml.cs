using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using TurismoRural_WEB.Services;

namespace TurismoRural_WEB.Pages
{
    public class ExperienciasReservarModel : PageModel
    {
        private readonly ExperienciasApiService _experienciasService;

        public ExperienciasReservarModel(ExperienciasApiService experienciasService)
        {
            _experienciasService = experienciasService;
        }

        public bool EsLoggedIn { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            var rolUsuario = HttpContext.Session.GetString("RolUsuario");

            EsLoggedIn = userId != null && (rolUsuario == "1" || rolUsuario == "2");

            System.Diagnostics.Debug.WriteLine($"[ExperienciasReservar] UserId: {userId}, RolUsuario: '{rolUsuario}'");

            // Si es admin, redirigir al panel de admin de experiencias
            if (rolUsuario == "1")
            {
                return RedirectToPage("/Experiencias");
            }

            System.Diagnostics.Debug.WriteLine($"[ExperienciasReservar] Acceso permitido para usuario {userId}");
            return Page();
        }

        // Obtener todas las experiencias base enriquecidas con nombre de comunidad
        public async Task<IActionResult> OnGetGetExperiencias()
        {
            try
            {
                var experiencias = await _experienciasService.GetExperienciasAsync();
                var resultado = new List<object>();

                foreach (var exp in experiencias)
                {
                    if (exp is not JsonElement je) continue;

                    // Leer campos con tolerancia a variaciones de casing
                    int expId = TryGetInt(je, "iD_Experiencia", "id_Experiencia", "ID_Experiencia", "id");
                    string titulo = TryGetString(je, "titulo", "Titulo", "TITULO");
                    string descripcion = TryGetString(je, "descripcion", "Descripcion", "DESCRIPCION");
                    string categoria = TryGetString(je, "categoria", "Categoria", "CATEGORIA");
                    int comunidadId = TryGetInt(je, "iD_Comunidad", "id_Comunidad", "ID_Comunidad");

                    string nombreComunidad = "Desconocida";
                    if (comunidadId > 0)
                    {
                        var comunidad = await _experienciasService.GetComunidadAsync(comunidadId);
                        if (comunidad is JsonElement comEl)
                            nombreComunidad = TryGetString(comEl, "nombre_Comunidad", "Nombre_Comunidad", "nombre") ?? "Desconocida";
                    }

                    resultado.Add(new
                    {
                        id = expId,
                        titulo,
                        descripcion,
                        categoria,
                        comunidad = nombreComunidad
                    });
                }

                return new JsonResult(resultado);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }

        private static int TryGetInt(JsonElement je, params string[] names)
        {
            foreach (var name in names)
                if (je.TryGetProperty(name, out var prop) && prop.TryGetInt32(out int val))
                    return val;
            return 0;
        }

        private static string TryGetString(JsonElement je, params string[] names)
        {
            foreach (var name in names)
                if (je.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
                    return prop.GetString() ?? string.Empty;
            return string.Empty;
        }
    }
}

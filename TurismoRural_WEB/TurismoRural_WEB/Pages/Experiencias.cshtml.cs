using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using TurismoRural_WEB.Services;

namespace TurismoRural_WEB.Pages
{
    public class ExperienciasModel : PageModel
    {
        private readonly ExperienciasApiService _experienciasService;

        public ExperienciasModel(ExperienciasApiService experienciasService)
        {
            _experienciasService = experienciasService;
        }

        public IActionResult OnGet()
        {
            // Validar que el usuario esté logueado
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Validar que el usuario tenga rol 1 (admin)
            var rolUsuario = HttpContext.Session.GetString("RolUsuario");
            if (rolUsuario != "1")
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        // Obtener todas las experiencias base enriquecidas con nombre de comunidad
        public async Task<IActionResult> OnGetGetExperiencias()
        {
            var experiencias = await _experienciasService.GetExperienciasAsync();

            // Enriquecer con nombres de comunidades
            var experienciasEnriquecidas = new List<Dictionary<string, object>>();

            foreach (var exp in experiencias)
            {
                var expDict = new Dictionary<string, object>();

                // Convertir JsonElement a diccionario
                if (exp is JsonElement jsonElement)
                {
                    // Copiar todas las propiedades del elemento JSON
                    foreach (var prop in jsonElement.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Number)
                        {
                            if (prop.Value.TryGetInt32(out int intValue))
                                expDict[prop.Name] = intValue;
                            else if (prop.Value.TryGetDecimal(out decimal decValue))
                                expDict[prop.Name] = decValue;
                        }
                        else if (prop.Value.ValueKind == JsonValueKind.String)
                        {
                            expDict[prop.Name] = prop.Value.GetString() ?? string.Empty;
                        }
                        else
                        {
                            expDict[prop.Name] = prop.Value.GetRawText();
                        }
                    }

                    // Obtener el ID de comunidad
                    int comunidadId = 0;
                    if (jsonElement.TryGetProperty("iD_Comunidad", out var idProp) && idProp.TryGetInt32(out int id))
                    {
                        comunidadId = id;
                    }

                    // Obtener el nombre de la comunidad
                    if (comunidadId > 0)
                    {
                        var comunidad = await _experienciasService.GetComunidadAsync(comunidadId);
                        if (comunidad is JsonElement comElement)
                        {
                            if (comElement.TryGetProperty("nombre_Comunidad", out var nombreProp))
                            {
                                expDict["nombre_Comunidad"] = nombreProp.GetString() ?? "Comunidad desconocida";
                            }
                        }
                    }
                }

                experienciasEnriquecidas.Add(expDict);
            }

            return new JsonResult(experienciasEnriquecidas);
        }

        public async Task<IActionResult> OnGetGetComunidades()
        {
            var comunidades = await _experienciasService.GetComunidadesAsync();
            return new JsonResult(comunidades);
        }

        public async Task<IActionResult> OnPostCreate([FromBody] System.Text.Json.JsonElement experiencia)
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null)
                return new JsonResult(new { success = false, error = "No autenticado" });

            var payload = new
            {
                titulo = experiencia.TryGetProperty("titulo", out var t) ? t.GetString() : null,
                descripcion = experiencia.TryGetProperty("descripcion", out var d) ? d.GetString() : null,
                categoria = experiencia.TryGetProperty("categoria", out var c) ? c.GetString() : null,
                usuarioIdRegistrador = userId.Value,
                iD_Comunidad = experiencia.TryGetProperty("iD_Comunidad", out var com) ? com.GetInt32() : 0
            };

            var result = await _experienciasService.CreateExperienciaAsync(payload);
            return new JsonResult(new { success = result });
        }
    }
}

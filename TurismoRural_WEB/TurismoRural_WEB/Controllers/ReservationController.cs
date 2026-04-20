using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using TurismoRural_WEB.Models.Reservation;
using Microsoft.AspNetCore.Http;


namespace TurismoRural_WEB.Controllers
{

    public class ReservationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReservationController(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

        private async Task<List<EstadoDto>> GetEstadosFromApiAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var estadosResponse = await client.GetAsync("https://localhost:7054/api/Catalog/estados");
                var estadosJson = await estadosResponse.Content.ReadAsStringAsync();

                if (!estadosResponse.IsSuccessStatusCode)
                {
                    return new List<EstadoDto>();
                }

                // Intentar deserializar directamente como lista
                try
                {
                    var estados = JsonSerializer.Deserialize<List<EstadoDto>>(estadosJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (estados != null && estados.Count > 0)
                        return estados;
                }
                catch
                {
                    // Si falla, intentar deserializar como objeto con propiedad "data" o "estados"
                    try
                    {
                        var wrapper = JsonSerializer.Deserialize<JsonElement>(estadosJson);
                        if (wrapper.ValueKind == JsonValueKind.Object)
                        {
                            // Buscar propiedades comunes que contengan los estados
                            foreach (var property in wrapper.EnumerateObject())
                            {
                                if (property.Value.ValueKind == JsonValueKind.Array)
                                {
                                    var estados = JsonSerializer.Deserialize<List<EstadoDto>>(property.Value.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                    if (estados != null && estados.Count > 0)
                                        return estados;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Silent fail
                    }
                }

                return new List<EstadoDto>();
            }
            catch
            {
                return new List<EstadoDto>();
            }
        }

            [HttpGet]
            public IActionResult Dashboard()
            {
                return View();
            }

            [HttpGet]
            public async Task<IActionResult> Create()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // Obtener concurrencias
                var concurrenciasResponse = await client.GetAsync("https://localhost:7054/api/ExperienciasConcurrencia");
                var concurrenciasJson = await concurrenciasResponse.Content.ReadAsStringAsync();
                var concurrencias = JsonSerializer.Deserialize<List<ConcurrenciaDto>>(concurrenciasJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ConcurrenciaDto>();

                // Obtener experiencias
                var experienciasResponse = await client.GetAsync("https://localhost:7054/api/Experiencias");
                var experienciasJson = await experienciasResponse.Content.ReadAsStringAsync();
                var experiencias = JsonSerializer.Deserialize<List<ExperienciaDto>>(experienciasJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ExperienciaDto>();

                // Obtener estados
                var estados = await GetEstadosFromApiAsync();

                // Asociar título de experiencia a cada concurrencia
                foreach (var c in concurrencias)
                {
                    var exp = experiencias.FirstOrDefault(e => e.ID_Experiencia == c.ID_Experiencia);
                    c.TituloExperiencia = exp?.Titulo ?? "Sin título";
                }

                var model = new ReservationCreateViewModel
                {
                    Concurrencias = concurrencias,
                    Estados = estados
                };
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las experiencias: " + ex.Message;
                return View(new ReservationCreateViewModel { Concurrencias = new List<ConcurrenciaDto>(), Estados = new List<EstadoDto>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReservationCreateViewModel model)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var concurrenciasResponse = await client.GetAsync("https://localhost:7054/api/ExperienciasConcurrencia");
                var concurrenciasJson = await concurrenciasResponse.Content.ReadAsStringAsync();
                var concurrencias = JsonSerializer.Deserialize<List<ConcurrenciaDto>>(concurrenciasJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ConcurrenciaDto>();

                var experienciasResponse = await client.GetAsync("https://localhost:7054/api/Experiencias");
                var experienciasJson = await experienciasResponse.Content.ReadAsStringAsync();
                var experiencias = JsonSerializer.Deserialize<List<ExperienciaDto>>(experienciasJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ExperienciaDto>();

                var estados = await GetEstadosFromApiAsync();

                foreach (var c in concurrencias)
                {
                    var exp = experiencias.FirstOrDefault(e => e.ID_Experiencia == c.ID_Experiencia);
                    c.TituloExperiencia = exp?.Titulo ?? "Sin título";
                }

                model.Concurrencias = concurrencias;
                model.Estados = estados;

                if (!ModelState.IsValid)
                    return View(model);

                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                    return RedirectToAction("Login", "Account");

                var reserva = new
                {
                    iD_Usuario = usuarioId,  // ← SIN el @ símbolo
                    iD_Concurrencia = model.ID_Concurrencia,
                    cantidad_Personas = model.Cantidad_Personas,
                    estado = model.Estado
                };

                var content = new StringContent(JsonSerializer.Serialize(reserva), Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("https://localhost:7054/api/Reservations", content);

                if (postResponse.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "¡Reserva creada correctamente!";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "No se pudo crear la reserva.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar la reserva: " + ex.Message);
                var client = _httpClientFactory.CreateClient();
                var concurrenciasResponse = await client.GetAsync("https://localhost:7054/api/ExperienciasConcurrencia");
                var concurrenciasJson = await concurrenciasResponse.Content.ReadAsStringAsync();
                var estados = await GetEstadosFromApiAsync();
                model.Concurrencias = JsonSerializer.Deserialize<List<ConcurrenciaDto>>(concurrenciasJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ConcurrenciaDto>();
                model.Estados = estados;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                    return RedirectToAction("Login", "Account");

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:7054/api/Reservations/by-user/{usuarioId}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudieron cargar las reservas.";
                    var emptyModel = new MyReservationsViewModel { Reservations = new List<Reservation>(), Estados = new List<EstadoDto>() };
                    return View(emptyModel);
                }

                var json = await response.Content.ReadAsStringAsync();
                var reservas = JsonSerializer.Deserialize<List<Reservation>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Reservation>();

                // Cargar estados
                var estados = await GetEstadosFromApiAsync();

                var model = new MyReservationsViewModel
                {
                    Reservations = reservas,
                    Estados = estados
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las reservas: " + ex.Message;
                var emptyModel = new MyReservationsViewModel { Reservations = new List<Reservation>(), Estados = new List<EstadoDto>() };
                return View(emptyModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchReservations()
        {
            var estados = await GetEstadosFromApiAsync();
            var model = new SearchReservationsViewModel { Estados = estados };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SearchReservations(int? reservaId)
        {
            try
            {
                if (!reservaId.HasValue || reservaId <= 0)
                {
                    ViewBag.Error = "Por favor ingresa un ID de reserva válido.";
                    var estados = await GetEstadosFromApiAsync();
                    var emptyModel = new SearchReservationsViewModel { Estados = estados };
                    return View(emptyModel);
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:7054/api/Reservations/{reservaId}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se encontró la reserva con ID {reservaId}.";
                    var estados = await GetEstadosFromApiAsync();
                    var emptyModel = new SearchReservationsViewModel { Estados = estados };
                    return View(emptyModel);
                }

                var json = await response.Content.ReadAsStringAsync();
                var reserva = JsonSerializer.Deserialize<Reservation>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Cargar estados
                var estadosData = await GetEstadosFromApiAsync();

                var model = new SearchReservationsViewModel
                {
                    Reservation = reserva,
                    Estados = estadosData
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al buscar la reserva: " + ex.Message;
                var estados = await GetEstadosFromApiAsync();
                var emptyModel = new SearchReservationsViewModel { Estados = estados };
                return View(emptyModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllReservations()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://localhost:7054/api/Reservations");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudieron cargar las reservas.";
                    var emptyModel = new AllReservationsViewModel { Reservations = new List<Reservation>(), Estados = new List<EstadoDto>() };
                    return View(emptyModel);
                }

                var json = await response.Content.ReadAsStringAsync();
                var reservas = JsonSerializer.Deserialize<List<Reservation>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Reservation>();

                // Cargar estados
                var estados = await GetEstadosFromApiAsync();

                var model = new AllReservationsViewModel
                {
                    Reservations = reservas,
                    Estados = estados
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las reservas: " + ex.Message;
                var emptyModel = new AllReservationsViewModel { Reservations = new List<Reservation>(), Estados = new List<EstadoDto>() };
                return View(emptyModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReservationStatus(int reservationId, int newStatus)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var updateData = new { estado = newStatus };
                var content = new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"https://localhost:7054/api/Reservations/{reservationId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["MensajeExito"] = "Estado de la reserva actualizado correctamente.";
                }
                else
                {
                    TempData["MensajeError"] = "No se pudo actualizar el estado de la reserva.";
                }

                return RedirectToAction("MyReservations");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al actualizar: " + ex.Message;
                return RedirectToAction("MyReservations");
            }
        }
    }
}
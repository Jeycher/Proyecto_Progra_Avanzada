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

        private static readonly List<EstadoDto> _estadosFallback = new()
        {
            new EstadoDto { iD_estado = 1, descripcion = "Pendiente" },
            new EstadoDto { iD_estado = 2, descripcion = "Confirmada" },
            new EstadoDto { iD_estado = 3, descripcion = "Cancelada" },
            new EstadoDto { iD_estado = 4, descripcion = "Completada" }
        };

        private async Task<List<EstadoDto>> GetEstadosFromApiAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("default");
                var estadosResponse = await client.GetAsync("https://localhost:7054/api/Catalog/estados");

                if (!estadosResponse.IsSuccessStatusCode)
                    return _estadosFallback;

                var estadosJson = await estadosResponse.Content.ReadAsStringAsync();
                var estados = JsonSerializer.Deserialize<List<EstadoDto>>(estadosJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return estados != null && estados.Count > 0 ? estados : _estadosFallback;
            }
            catch
            {
                return _estadosFallback;
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
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path + Request.QueryString });

            try
            {
                var client = _httpClientFactory.CreateClient("default");

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
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var client = _httpClientFactory.CreateClient("default");

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
                var client = _httpClientFactory.CreateClient("default");
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

                var client = _httpClientFactory.CreateClient("default");
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
                    Estados = estados,
                    EsAdmin = HttpContext.Session.GetString("RolUsuario") == "1"
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
        public async Task<IActionResult> SearchReservations(int? id)
        {
            var estados = await GetEstadosFromApiAsync();

            if (!id.HasValue || id <= 0)
            {
                var emptyModel = new SearchReservationsViewModel { Estados = estados };
                return View(emptyModel);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("default");
                var response = await client.GetAsync($"https://localhost:7054/api/Reservations/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se encontró la reserva con ID {id}.";
                    return View(new SearchReservationsViewModel { Estados = estados });
                }

                var json = await response.Content.ReadAsStringAsync();
                var reserva = JsonSerializer.Deserialize<Reservation>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(new SearchReservationsViewModel { Reservation = reserva, Estados = estados });
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al buscar la reserva: " + ex.Message;
                return View(new SearchReservationsViewModel { Estados = estados });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllReservations()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("default");
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
        public async Task<IActionResult> UpdateReservationStatus(
            int reservationId, int newStatus,
            int userId, int concurrenciaId, int cantidadPersonas,
            string returnTo = "MyReservations")
        {
            try
            {
                var client = _httpClientFactory.CreateClient("default");

                var updateData = new
                {
                    iD_Usuario        = userId,
                    iD_Concurrencia   = concurrenciaId,
                    cantidad_Personas = cantidadPersonas,
                    estado            = newStatus
                };

                var content = new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json");
                var putResponse = await client.PutAsync($"https://localhost:7054/api/Reservations/{reservationId}", content);
                var responseBody = await putResponse.Content.ReadAsStringAsync();

                if (putResponse.IsSuccessStatusCode || putResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["MensajeExito"] = "Estado actualizado correctamente.";
                }
                else
                {
                    TempData["MensajeError"] = $"Error {(int)putResponse.StatusCode}: {responseBody}";
                }

                return RedirectToAction(returnTo);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al actualizar: " + ex.Message;
                return RedirectToAction(returnTo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("default");
                var response = await client.DeleteAsync($"https://localhost:7054/api/Reservations/{reservationId}");

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    TempData["MensajeExito"] = "Reserva cancelada correctamente.";
                else
                    TempData["MensajeError"] = $"No se pudo cancelar la reserva (Error {(int)response.StatusCode}).";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al cancelar: " + ex.Message;
            }

            return RedirectToAction("MyReservations");
        }
    }
}
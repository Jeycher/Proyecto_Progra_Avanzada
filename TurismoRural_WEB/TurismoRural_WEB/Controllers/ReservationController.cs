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

                // Asociar título de experiencia a cada concurrencia
                foreach (var c in concurrencias)
                {
                    var exp = experiencias.FirstOrDefault(e => e.ID_Experiencia == c.ID_Experiencia);
                    c.TituloExperiencia = exp?.Titulo ?? "Sin título";
                }

                var model = new ReservationCreateViewModel
                {
                    Concurrencias = concurrencias
                };
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las experiencias: " + ex.Message;
                return View(new ReservationCreateViewModel { Concurrencias = new List<ConcurrenciaDto>() });
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

                foreach (var c in concurrencias)
                {
                    var exp = experiencias.FirstOrDefault(e => e.ID_Experiencia == c.ID_Experiencia);
                    c.TituloExperiencia = exp?.Titulo ?? "Sin título";
                }

                model.Concurrencias = concurrencias;

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
                model.Concurrencias = JsonSerializer.Deserialize<List<ConcurrenciaDto>>(concurrenciasJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ConcurrenciaDto>();
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
                    return View(new List<Reservation>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var reservas = JsonSerializer.Deserialize<List<Reservation>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Reservation>();

                return View(reservas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las reservas: " + ex.Message;
                return View(new List<Reservation>());
            }
        }

        [HttpGet]
        public IActionResult SearchReservations()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchReservations(int? reservaId)
        {
            try
            {
                if (!reservaId.HasValue || reservaId <= 0)
                {
                    ViewBag.Error = "Por favor ingresa un ID de reserva válido.";
                    return View();
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:7054/api/Reservations/{reservaId}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"No se encontró la reserva con ID {reservaId}.";
                    return View();
                }

                var json = await response.Content.ReadAsStringAsync();
                var reserva = JsonSerializer.Deserialize<Reservation>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(reserva);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al buscar la reserva: " + ex.Message;
                return View();
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
                    return View(new List<Reservation>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var reservas = JsonSerializer.Deserialize<List<Reservation>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Reservation>();

                return View(reservas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar las reservas: " + ex.Message;
                return View(new List<Reservation>());
            }
        }
    }
}
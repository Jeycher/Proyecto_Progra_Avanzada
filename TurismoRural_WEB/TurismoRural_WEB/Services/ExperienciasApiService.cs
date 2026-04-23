using System.Text;
using System.Text.Json;

namespace TurismoRural_WEB.Services
{
    public class ExperienciasApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ExperienciasApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // ============ EXPERIENCIAS BASE (Solo lectura) ============

        public async Task<List<dynamic>> GetExperienciasAsync()
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + "experiencias";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return new List<dynamic>();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<dynamic>>(json, options) ?? new List<dynamic>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetExperienciasAsync: {ex.Message}");
                return new List<dynamic>();
            }
        }

        // ============ COMUNIDADES ============

        public async Task<dynamic> GetComunidadAsync(int id)
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + $"Comunidades/{id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<dynamic>(json, options);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetComunidadAsync: {ex.Message}");
                return null;
            }
        }

        // ============ EXPERIENCIAS CONCURRENCIA ============

        public async Task<List<dynamic>> GetConcurrenciasAsync()
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + "experienciasconcurrencia";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return new List<dynamic>();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<dynamic>>(json, options) ?? new List<dynamic>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetConcurrenciasAsync: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<bool> CreateConcurrenciaAsync(dynamic concurrencia)
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + "experienciasconcurrencia";
                var json = JsonSerializer.Serialize(concurrencia);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en CreateConcurrenciaAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateConcurrenciaAsync(int id, dynamic concurrencia)
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + $"experienciasconcurrencia/{id}";
                var json = JsonSerializer.Serialize(concurrencia);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(apiUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en UpdateConcurrenciaAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteConcurrenciaAsync(int id)
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + $"experienciasconcurrencia/{id}";
                var response = await _httpClient.DeleteAsync(apiUrl);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DeleteConcurrenciaAsync: {ex.Message}");
                return false;
            }
        }

        // ============ RESERVATIONS (Reservas de Experiencias) ============

        public async Task<bool> CreateReservationAsync(dynamic reservation)
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + "reservations";
                var json = JsonSerializer.Serialize(reservation);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en CreateReservationAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<dynamic>> GetReservationsByUserAsync(int userId)
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + $"reservations/by-user/{userId}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return new List<dynamic>();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<List<dynamic>>(json, options) ?? new List<dynamic>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetReservationsByUserAsync: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            try
            {
                var apiUrl = _configuration["Valores:UrlAPI"] + $"reservations/{id}";
                var response = await _httpClient.DeleteAsync(apiUrl);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DeleteReservationAsync: {ex.Message}");
                return false;
            }
        }
    }
}

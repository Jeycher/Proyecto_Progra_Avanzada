using System.Text;
using System.Text.Json;
using TurismoRural_WEB.Models;

namespace TurismoRural_WEB.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUserAsync(UserRegisterViewModel model)
        {
            var apiUrl = _configuration["Valores:UrlAPI"] + "Users/RegistroUsuario";

            var payload = new
            {
                nombre = model.Nombre,
                correoElectronico = model.CorreoElectronico,
                contrasenna = model.Contrasena,
                telefono = model.Telefono
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<UsuarioResponse?> LoginUserAsync(LoginViewModel model)
        {
            var apiUrl = _configuration["Valores:UrlAPI"] + "Users/IniciarSesion";

            var payload = new
            {
                correoElectronico = model.Correo,
                contrasenna = model.Contrasena
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UsuarioResponse>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<List<UserViewModel>> GetUsersAsync()
        {
            var apiUrl = _configuration["Valores:UrlAPI"] + "Users";

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return new List<UserViewModel>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<UserViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<UserViewModel>();
        }

        public async Task<UserEditViewModel?> GetUserByIdAsync(int id)
        {
            var apiUrl = _configuration["Valores:UrlAPI"] + $"Users/{id}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UserEditViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<bool> UpdateUserAsync(UserEditViewModel model)
        {
            var apiUrl = _configuration["Valores:UrlAPI"] + $"Users/{model.id_Usuario}";

            var payload = new
            {
                id_Usuario = model.id_Usuario,
                nombre = model.nombre,
                correo = model.correo,
                telefono = model.telefono,
                id_Rol = model.id_Rol
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var apiUrl = _configuration["Valores:UrlAPI"] + $"Users/{id}";
            var response = await _httpClient.DeleteAsync(apiUrl);

            return response.IsSuccessStatusCode;
        }
    }
}




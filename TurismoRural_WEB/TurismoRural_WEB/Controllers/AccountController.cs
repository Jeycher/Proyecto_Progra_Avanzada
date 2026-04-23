using Microsoft.AspNetCore.Mvc;
using TurismoRural_WEB.Models;
using TurismoRural_WEB.Services;

namespace TurismoRural_WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserApiService _userApiService;

        public AccountController(UserApiService userApiService)
        {
            _userApiService = userApiService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _userApiService.LoginUserAsync(model);

            if (response == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View(model);
            }

            // Save user data to session variables
            HttpContext.Session.SetInt32("UsuarioId", response.Id); // este es el id del usaurio que esta loguiado 
            HttpContext.Session.SetString("NombreUsuario", response.Nombre);
            HttpContext.Session.SetString("CorreoUsuario", response.Correo);
            HttpContext.Session.SetString("TokenUsuario", response.Token);

            // Guardar el rol como string "1" o "2"
            var rolString = response.Rol.ToString();
            HttpContext.Session.SetString("RolUsuario", rolString);

            System.Diagnostics.Debug.WriteLine($"[LOGIN] Usuario: {response.Nombre}, Rol guardado: {rolString}");

            HttpContext.Session.SetString("RegistroExitoso", "true");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userApiService.RegisterUserAsync(model);

            if (result)
            {
                TempData["Mensaje"] = "Usuario registrado correctamente. Por favor inicie sesión.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "No se pudo registrar el usuario.";
            return View(model);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var nombre = HttpContext.Session.GetString("NombreUsuario");

            if (string.IsNullOrEmpty(nombre))
                return RedirectToAction("Login");

            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

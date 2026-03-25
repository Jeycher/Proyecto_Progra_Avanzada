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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userApiService.LoginUserAsync(model);

            if (user == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View(model);
            }

            HttpContext.Session.SetString("NombreUsuario", user.nombre);
            HttpContext.Session.SetString("CorreoUsuario", user.correo);
            HttpContext.Session.SetString("RolUsuario", user.id_Rol.ToString());

            return RedirectToAction("Profile");
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
                TempData["Mensaje"] = "Usuario registrado correctamente.";
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

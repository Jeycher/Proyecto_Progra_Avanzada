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
        public async Task<IActionResult> Profile()
        {
            var users = await _userApiService.GetUsersAsync();
            return View(users);
        }
    }
}

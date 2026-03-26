using Microsoft.AspNetCore.Mvc;
using TurismoRural_WEB.Services;

namespace TurismoRural_WEB.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserApiService _userApiService;

        public AdminController(UserApiService userApiService)
        {
            _userApiService = userApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var rol = HttpContext.Session.GetString("RolUsuario");

            if (string.IsNullOrEmpty(rol))
                return RedirectToAction("Login", "Account");

            if (rol != "1")
                return RedirectToAction("Profile", "Account");

            var users = await _userApiService.GetUsersAsync();
            return View(users);
        }
    }
}

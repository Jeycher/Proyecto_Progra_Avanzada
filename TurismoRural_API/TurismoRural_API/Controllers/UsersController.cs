using Microsoft.AspNetCore.Mvc;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;
using TurismoRural_API.Repositories;
using TurismoRural_API.Utilities;

namespace TurismoRural_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly DapperContext _context;

        public UsersController(IUserRepository userRepository, DapperContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(UsersController) + ".GetAll", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(UsersController) + ".GetById", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null) return BadRequest();
            try
            {
                // In a real app, hash the password and validate fields
                var existing = await _userRepository.GetByEmailAsync(user.Correo);
                if (existing != null) return Conflict("Email already registered.");

                var id = await _userRepository.CreateAsync(user);
                user.ID_Usuario = id;
                return CreatedAtAction(nameof(GetById), new { id = id }, user);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(UsersController) + ".Register", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}

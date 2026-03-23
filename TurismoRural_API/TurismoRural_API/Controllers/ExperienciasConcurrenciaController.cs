using Microsoft.AspNetCore.Mvc;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;
using TurismoRural_API.Repositories;
using TurismoRural_API.Utilities;

namespace TurismoRural_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExperienciasConcurrenciaController : ControllerBase
    {
        private readonly IExperienciaConcurrenciaRepository _repo;
        private readonly DapperContext _context;

        public ExperienciasConcurrenciaController(IExperienciaConcurrenciaRepository repo, DapperContext context)
        {
            _repo = repo;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _repo.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(ExperienciasConcurrenciaController) + ".GetAll", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var item = await _repo.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(item);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(ExperienciasConcurrenciaController) + ".Get", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExperienciaConcurrenciaCreateRequest model)
        {
            if (model == null) return BadRequest();

            try
            {
                var id = await _repo.CreateAsync(model);
                var creado = await _repo.GetByIdAsync(id);
                return Ok(creado);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(ExperienciasConcurrenciaController) + ".Create", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExperienciaConcurrenciaUpdateRequest model)
        {
            if (model == null) return BadRequest();

            try
            {
                var ok = await _repo.UpdateAsync(id, model);
                if (!ok) return NotFound();

                var actualizado = await _repo.GetByIdAsync(id);
                return Ok(actualizado);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(ExperienciasConcurrenciaController) + ".Update", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _repo.DeleteAsync(id);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(ExperienciasConcurrenciaController) + ".Delete", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}

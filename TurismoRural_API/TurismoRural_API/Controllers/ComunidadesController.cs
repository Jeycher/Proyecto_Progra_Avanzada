using Microsoft.AspNetCore.Mvc;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;
using TurismoRural_API.Repositories;
using TurismoRural_API.Utilities;

namespace TurismoRural_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComunidadesController : ControllerBase
    {
        private readonly IComunidadRepository _repo;
        private readonly DapperContext _context;

        public ComunidadesController(IComunidadRepository repo, DapperContext context)
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
                await ErrorLogger.LogAsync(_context, nameof(ComunidadesController) + ".GetAll", ex.Message, ex.StackTrace);
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
                await ErrorLogger.LogAsync(_context, nameof(ComunidadesController) + ".Get", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ComunidadCreateRequest model)
        {
            if (model == null) return BadRequest();

            try
            {
                var comunidad = new Comunidad
                {
                    Nombre_Comunidad = model.Nombre_Comunidad,
                    Pais = model.Pais,
                    Provincia = model.Provincia,
                    Descripcion = model.Descripcion
                };

                var id = await _repo.CreateAsync(comunidad);
                comunidad.ID_Comunidad = id;
                return Ok(comunidad);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(ComunidadesController) + ".Create", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ComunidadUpdateRequest model)
        {
            if (model == null) return BadRequest();

            try
            {
                var comunidad = new Comunidad
                {
                    ID_Comunidad = id,
                    Nombre_Comunidad = model.Nombre_Comunidad,
                    Pais = model.Pais,
                    Provincia = model.Provincia,
                    Descripcion = model.Descripcion
                };

                var ok = await _repo.UpdateAsync(comunidad);
                if (!ok) return NotFound();
                return Ok(comunidad);
            }
            catch (Exception ex)
            {
                await ErrorLogger.LogAsync(_context, nameof(ComunidadesController) + ".Update", ex.Message, ex.StackTrace);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;

namespace TurismoRural_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _repository;

        public CatalogController(ICatalogRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all reservation status values from CatalogoEstado
        /// </summary>
        /// <returns>List of status options (Pendiente, Confirmada, Cancelada, Completada)</returns>
        [HttpGet("estados")]
        public async Task<ActionResult<IEnumerable<CatalogoEstado>>> GetEstados()
        {
            try
            {
                var estados = await _repository.GetAllEstadosAsync();
                return Ok(estados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving status catalog", error = ex.Message });
            }
        }
    }
}

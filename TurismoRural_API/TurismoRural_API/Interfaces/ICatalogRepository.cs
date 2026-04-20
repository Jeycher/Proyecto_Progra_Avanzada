using TurismoRural_API.Models;

namespace TurismoRural_API.Interfaces
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<CatalogoEstado>> GetAllEstadosAsync();
    }
}

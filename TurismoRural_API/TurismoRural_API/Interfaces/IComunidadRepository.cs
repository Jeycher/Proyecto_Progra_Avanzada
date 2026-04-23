using TurismoRural_API.Models;

namespace TurismoRural_API.Interfaces
{
    public interface IComunidadRepository
    {
        Task<IEnumerable<Comunidad>> GetAllAsync();
        Task<Comunidad?> GetByIdAsync(int id);
        Task<int> CreateAsync(Comunidad comunidad);
        Task<bool> UpdateAsync(Comunidad comunidad);
    }
}

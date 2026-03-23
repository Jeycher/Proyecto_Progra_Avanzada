using TurismoRural_API.Models;

namespace TurismoRural_API.Interfaces
{
    public interface IExperienciaRepository
    {
        Task<IEnumerable<Experiencia>> GetAllAsync();
        Task<Experiencia?> GetByIdAsync(int id);
        Task<int> CreateAsync(ExperienciaCreateRequest model);
        Task<bool> UpdateAsync(int id, ExperienciaUpdateRequest model);
        Task<bool> DeleteAsync(int id);
    }
}

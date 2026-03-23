using TurismoRural_API.Models;

namespace TurismoRural_API.Interfaces
{
    public interface IExperienciaConcurrenciaRepository
    {
        Task<IEnumerable<ExperienciaConcurrencia>> GetAllAsync();
        Task<ExperienciaConcurrencia?> GetByIdAsync(int id);
        Task<int> CreateAsync(ExperienciaConcurrenciaCreateRequest model);
        Task<bool> UpdateAsync(int id, ExperienciaConcurrenciaUpdateRequest model);
        Task<bool> DeleteAsync(int id);
    }
}

using TurismoRural_API.Models;

namespace TurismoRural_API.Interfaces
{
    public interface IExperienceRepository
    {
        Task<int> CreateAsync(Experience experience);
        Task<IEnumerable<Experience>> GetAllAsync();
        Task<Experience?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Experience experience);
        Task<bool> DeleteAsync(int id);
    }
}
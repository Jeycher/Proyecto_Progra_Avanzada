using TurismoRural_API.Models;

namespace TurismoRural_API.Interfaces
{
    public interface IReservationRepository
    {
        Task<int> CreateAsync(CreateReservationDto dto);
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);
        Task<bool> UpdateAsync(int id, UpdateReservationDto model);
        Task<bool> DeleteAsync(int id);
    }
}
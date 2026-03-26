using TurismoRural_API.Models.Reservas;

namespace TurismoRural_API.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<int> CreateAsync(CreateReservationDto dto);
        Task<Reservation?> GetByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);
        Task<bool> UpdateAsync(int id, UpdateReservationDto model);
        Task<bool> DeleteAsync(int id);
    }
}
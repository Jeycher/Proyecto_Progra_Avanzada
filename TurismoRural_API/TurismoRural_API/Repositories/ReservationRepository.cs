using Dapper;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;

namespace TurismoRural_API.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DapperContext _context;

        public ReservationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Reservation reservation)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ExperienceId", reservation.ExperienceId);
            parameters.Add("@UserId", reservation.UserId);
            parameters.Add("@DateFrom", reservation.DateFrom);
            parameters.Add("@DateTo", reservation.DateTo);
            parameters.Add("@Status", reservation.Status);
            parameters.Add("@CreatedAt", reservation.CreatedAt);

            var id = await connection.QuerySingleAsync<int>("sp_CrearReserva", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            var affected = await connection.ExecuteAsync("sp_EliminarReserva", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return affected > 0;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Reservation>("sp_ObtenerReservas", commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return await connection.QueryFirstOrDefaultAsync<Reservation>("sp_ObtenerReservaPorId", parameters, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            return await connection.QueryAsync<Reservation>("sp_ObtenerReservasPorUsuario", parameters, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateAsync(Reservation reservation)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", reservation.Id);
            parameters.Add("@ExperienceId", reservation.ExperienceId);
            parameters.Add("@UserId", reservation.UserId);
            parameters.Add("@DateFrom", reservation.DateFrom);
            parameters.Add("@DateTo", reservation.DateTo);
            parameters.Add("@Status", reservation.Status);

            var affected = await connection.ExecuteAsync("sp_ActualizarReserva", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return affected > 0;
        }
    }
}
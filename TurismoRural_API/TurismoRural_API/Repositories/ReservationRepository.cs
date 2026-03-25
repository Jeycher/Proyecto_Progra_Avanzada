using Dapper;
using System.Data;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;
using TurismoRural_API.Models.Reservas;

namespace TurismoRural_API.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DapperContext _context;

        public ReservationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CreateReservationDto dto)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.ExecuteScalarAsync<int>(
                "sp_CrearReserva",
                new
                {
                    Estado = 1,
                    ID_Usuario = dto.UserId,
                    ID_Fecha = dto.FechaId,
                    Cantidad_Personas = dto.CantidadPersonas,
                    Fecha_Reserva = DateTime.Now.ToString("yyyy-MM-dd")
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ID_Reserva", id);
            var affected = await connection.ExecuteAsync("DELETE FROM Reserva WHERE ID_Reserva = @ID_Reserva;", parameters);
            return affected > 0;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Reservation>(
                "SP_ConsultarReservas",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ID_Reserva", id);

            return await connection.QueryFirstOrDefaultAsync<Reservation>(
                "sp_ObtenerReservaPorID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ID_Usuario", userId);

            return await connection.QueryAsync<Reservation>(
                "SP_ObtenerReservasPorUsuario",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> UpdateAsync(int id, UpdateReservationDto model)
        {
            using var connection = _context.CreateConnection();

            var result = await connection.ExecuteAsync(
                "sp_ActualizarReserva",
                new
                {
                    ID_Reserva = id,
                    ID_Usuario = model.ID_Usuario,
                    ID_Fecha = model.ID_Fecha,
                    Cantidad_Personas = model.Cantidad_Personas,
                    Estado = model.Estado
                },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
    }
}
using Dapper;
using TurismoRural_API.Interfaces;
using TurismoRural_API.Models;

namespace TurismoRural_API.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly DapperContext _context;

        public CatalogRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CatalogoEstado>> GetAllEstadosAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<CatalogoEstado>(
                "SP_ConsultarCatalogoEstado",
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}

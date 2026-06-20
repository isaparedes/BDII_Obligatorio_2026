using Dapper;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class EntradaRepository
{
    private readonly DatabaseConnection _db;

    public EntradaRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Obtener una entrada por el mail de su usuario_general titular
    public async Task<IEnumerable<EntradaResponseDTO>> ObtenerPorTitular(string mailTitular)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<EntradaResponseDTO>(
            "SELECT * FROM entrada WHERE mail_titular = @MailTitular",
            new { MailTitular = mailTitular }
        );
    }

    // Obtener una entrada por su id_entrada
    public async Task<EntradaResponseDTO?> ObtenerPorId(int idEntrada)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<EntradaResponseDTO>(
            "SELECT * FROM entrada WHERE id_entrada = @IdEntrada",
            new { IdEntrada = idEntrada }
        );
    }

}
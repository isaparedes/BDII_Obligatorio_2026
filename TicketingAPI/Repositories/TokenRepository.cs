using Dapper;
using TicketingAPI.Database;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class TokenRepository
{
    private readonly DatabaseConnection _db;

    public TokenRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public async Task RegenerarTokens()
    {
        using var conn = _db.CreateConnection();

        // Expirar tokens vencidos
        await conn.ExecuteAsync(@"
            UPDATE token
            SET estado_token = 'Expirado'
            WHERE estado_token = 'Activo'
            AND fecha_hora_expiracion < NOW()"
        );

        // Obtener entradas activas sin token activo
        var entradas = await conn.QueryAsync<int>(@"
            SELECT e.id_entrada
            FROM entrada e
            INNER JOIN evento ev ON e.id_evento = ev.id_evento
            WHERE e.estado_entrada = 'Emitida'
            AND CONCAT(ev.fecha_evento, ' ', ev.hora_evento) > NOW()
            AND NOT EXISTS (
                SELECT 1 FROM token t
                WHERE t.id_entrada = e.id_entrada
                AND t.estado_token = 'Activo'
            )"
        );

        var ahora = DateTime.Now;
        foreach (var idEntrada in entradas)
        {
            await conn.ExecuteAsync(@"
                INSERT INTO token
                (codigo_qr, estado_token, fecha_hora_vigencia, fecha_hora_expiracion, id_entrada)
                VALUES
                (@CodigoQr, 'Activo', @Vigencia, DATE_ADD(@Vigencia, INTERVAL 30 SECOND), @IdEntrada)",
                new
                {
                    CodigoQr = Guid.NewGuid().ToString(),
                    Vigencia = ahora,
                    IdEntrada = idEntrada
                }
            );
        }

        Console.WriteLine($"[{DateTime.Now}] Tokens regenerados para {entradas.Count()} entradas");
    }

    public async Task<Token?> ObtenerTokenActivo(int idEntrada)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Token>(@"
            SELECT * FROM token
            WHERE id_entrada = @IdEntrada
            AND estado_token = 'Activo'",
            new { IdEntrada = idEntrada }
        );
    }
}
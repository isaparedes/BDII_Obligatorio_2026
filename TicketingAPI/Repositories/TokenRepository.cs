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
 
        // Solo entradas Emitidas con evento futuro y token Activo
        var tokens = await conn.QueryAsync<Token>(
        @"
        SELECT
            t.codigo_qr              AS CodigoQr,
            t.estado_token           AS EstadoToken,
            t.fecha_hora_vigencia    AS FechaHoraVigencia,
            t.fecha_hora_expiracion  AS FechaHoraExpiracion,
            t.fecha_hora_validacion  AS FechaHoraValidacion,
            t.id_entrada             AS IdEntrada,
            t.id_dispositivo_valida  AS IdDispositivoValida
        FROM token t
        INNER JOIN entrada e  ON t.id_entrada  = e.id_entrada
        INNER JOIN evento  ev ON e.id_evento   = ev.id_evento
        WHERE
            t.estado_token    = 'Activo'
            AND e.estado_entrada = 'Emitida'
            AND CONCAT(ev.fecha_evento, ' ', ev.hora_evento) > NOW()
        ");
 
        foreach (var token in tokens)
        {
            // Expira token anterior
            await conn.ExecuteAsync(
            @"
            UPDATE token
            SET estado_token = 'Expirado'
            WHERE codigo_qr = @CodigoQr
            ",
            new { token.CodigoQr });
 
            // Genera nuevo token — se usa la misma base de tiempo
            // para que el CHECK de la BD (expiracion = vigencia + 30s) se cumpla
            var ahora = DateTime.Now;
            var nuevoToken = new Token
            {
                CodigoQr             = Guid.NewGuid().ToString(),
                EstadoToken          = "Activo",
                FechaHoraVigencia    = ahora,
                FechaHoraExpiracion  = ahora.AddSeconds(30),
                IdEntrada            = token.IdEntrada
            };
 
            await conn.ExecuteAsync(
            @"
            INSERT INTO token
            (
                codigo_qr,
                estado_token,
                fecha_hora_vigencia,
                fecha_hora_expiracion,
                id_entrada
            )
            VALUES
            (
                @CodigoQr,
                @EstadoToken,
                @FechaHoraVigencia,
                @FechaHoraExpiracion,
                @IdEntrada
            )
            ",
            nuevoToken);
        }
    }
}
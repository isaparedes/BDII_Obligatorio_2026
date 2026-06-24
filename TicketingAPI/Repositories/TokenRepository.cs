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

    // Obtener un token activo o generar uno nuevo para utilizar en una validación
    public async Task<Token> ObtenerOGenerarTokenActivo(int idEntrada)
    {
        using var conn = _db.CreateConnection();

        var tokenActivo = await conn.QueryFirstOrDefaultAsync<Token>(@"
            SELECT * FROM token
            WHERE id_entrada = @IdEntrada
            AND estado_token = 'Activo'
            AND fecha_hora_expiracion > NOW()",
            new { IdEntrada = idEntrada }
        );

        if (tokenActivo != null)
            return tokenActivo;

        await conn.ExecuteAsync(@"
            UPDATE token SET estado_token = 'Expirado'
            WHERE id_entrada = @IdEntrada
            AND estado_token = 'Activo'",
            new { IdEntrada = idEntrada }
        );

        var ahora = DateTime.Now;
        var codigoQr = Guid.NewGuid().ToString();

        await conn.ExecuteAsync(@"
            INSERT INTO token
            (codigo_qr, estado_token, fecha_hora_vigencia, fecha_hora_expiracion, id_entrada)
            VALUES
            (@CodigoQr, 'Activo', @Vigencia, DATE_ADD(@Vigencia, INTERVAL 30 SECOND), @IdEntrada)",
            new { CodigoQr = codigoQr, Vigencia = ahora, IdEntrada = idEntrada }
        );

        var token = await conn.QueryFirstOrDefaultAsync<Token>(
            "SELECT * FROM token WHERE codigo_qr = @CodigoQr",
            new { CodigoQr = codigoQr }
        );

        if (token == null)
            throw new Exception("No se pudo crear el token");

        return token;
    }
}
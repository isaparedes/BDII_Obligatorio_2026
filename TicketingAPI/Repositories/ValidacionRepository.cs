using Dapper;
using TicketingAPI.Database;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class ValidacionRepository
{
    private readonly DatabaseConnection _db;

    public ValidacionRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Validar un token de entrada
    public async Task<string> ValidarToken(ValidarTokenDTO dto)
    {
        using var conn = _db.CreateConnection();


        var token = await conn.QueryFirstOrDefaultAsync<Token>(
            "SELECT * FROM token WHERE codigo_qr = @CodigoQr",
            new { dto.CodigoQr }
        );

        if (token == null)
            return "Token inválido";

        if (token.EstadoToken != "Activo")
            return "Token expirado o ya utilizado";

        if (token.FechaHoraExpiracion < DateTime.Now)
            return "Token expirado";

        var entrada = await conn.QueryFirstOrDefaultAsync<Entrada>(
            "SELECT * FROM entrada WHERE id_entrada = @IdEntrada",
            new { token.IdEntrada }
        );

        if (entrada != null && entrada.EstadoEntrada == "Consumida")
            return "La entrada ya fue consumida";

        var autorizado = await conn.QueryFirstOrDefaultAsync<int>(@"
            SELECT COUNT(*) FROM asignacion a
            INNER JOIN dispositivo d ON a.mail_funcionario = d.mail_funcionario
            WHERE d.id_dispositivo = @IdDispositivo
            AND a.id_evento = @IdEvento",
            new { dto.IdDispositivo, entrada!.IdEvento }
        );

        if (autorizado == 0)
            return "El dispositivo no está autorizado para este sector en ese evento";

        await conn.ExecuteAsync(@"
            UPDATE token
            SET id_dispositivo_valida = @IdDispositivo,
                fecha_hora_validacion = NOW(),
                estado_token = 'Expirado'
            WHERE codigo_qr = @CodigoQr",
            new { dto.IdDispositivo, dto.CodigoQr }
        );

        return "Acceso permitido";
    }
}
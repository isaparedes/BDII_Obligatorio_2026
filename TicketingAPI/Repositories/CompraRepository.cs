using Dapper;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class CompraRepository
{
    private readonly DatabaseConnection _db;

    public CompraRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Crear una nueva compra
    public async Task<int> CrearCompra(string mailComprador)
    {
        using var conn = _db.CreateConnection();

        var idComision = await conn.QueryFirstOrDefaultAsync<int>(@"
            SELECT id_comision FROM comision
            WHERE fecha_inicio <= CURDATE() AND fecha_fin >= CURDATE()
            LIMIT 1"
        );

        if (idComision == 0)
            throw new Exception("No hay comisión vigente");

        var idCompra = await conn.QueryFirstOrDefaultAsync<int>(@"
            INSERT INTO compra (fecha_compra, estado_compra, monto_total, mail_comprador, id_comision)
            VALUES (CURDATE(), 'Pendiente', 1, @MailComprador, @IdComision);
            SELECT LAST_INSERT_ID();",
            new { MailComprador = mailComprador, IdComision = idComision }
        );

        return idCompra;
    }

    // Agregar una entrada a una compra
    public async Task AgregarEntrada(int idCompra, EntradaItemDTO entrada)
    {
        using var conn = _db.CreateConnection();

        var evento = await conn.QueryFirstOrDefaultAsync<Evento>(@"
            SELECT id_estadio, fecha_evento FROM evento
            WHERE id_evento = @IdEvento",
            new { entrada.IdEvento }
        );

        if (evento == null)
            throw new Exception("El evento no existe");

        if (evento.FechaEvento <= DateTime.Today)
            throw new Exception("No se pueden comprar entradas para un evento que ya ocurrió");

        // Verificar que el sector está habilitado para ese evento
        var sectorHabilitado = await conn.QueryFirstOrDefaultAsync<int>(@"
            SELECT COUNT(*) FROM habilita
            WHERE id_evento = @IdEvento
            AND id_estadio = @IdEstadio
            AND nombre_sector = @NombreSector",
            new { entrada.IdEvento, evento.IdEstadio, entrada.NombreSector }
        );

        if (sectorHabilitado == 0)
            throw new Exception("El sector no está habilitado para este evento");

        var costoEntrada = await conn.QueryFirstOrDefaultAsync<decimal>(@"
            SELECT costo_sector FROM sector
            WHERE id_estadio = @IdEstadio AND nombre_sector = @NombreSector",
            new { evento.IdEstadio, entrada.NombreSector }
        );

        if (costoEntrada == 0)
            throw new Exception("Sector no encontrado");

        await conn.ExecuteAsync(@"
            INSERT INTO entrada 
            (costo_entrada, estado_entrada, id_compra, id_evento, id_estadio, nombre_sector, mail_titular)
            VALUES 
            (@CostoEntrada, 'Emitida', @IdCompra, @IdEvento, @IdEstadio, @NombreSector, '')",
            new
            {
                CostoEntrada = costoEntrada,
                IdCompra = idCompra,
                entrada.IdEvento,
                IdEstadio = evento.IdEstadio,
                entrada.NombreSector
            }
        );

        var idEntrada = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT LAST_INSERT_ID()"
        );

        var ahora = DateTime.Now;
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

    // Confirmar "pago" de compra
    public async Task PagarCompra(int idCompra)
    {
        using var conn = _db.CreateConnection();

        // Obtener suma de entradas y valor de comision
        var resultado = await conn.QueryFirstOrDefaultAsync<dynamic>(@"
            SELECT 
                SUM(e.costo_entrada) AS subtotal,
                co.valor_comision
            FROM entrada e
            JOIN compra c ON e.id_compra = c.id_compra
            JOIN comision co ON c.id_comision = co.id_comision
            WHERE e.id_compra = @IdCompra",
            new { IdCompra = idCompra }
        );

        decimal subtotal = resultado.subtotal;
        decimal comision = resultado.valor_comision;
        decimal montoFinal = subtotal * (1 + comision / 100);

        await conn.ExecuteAsync(@"
            UPDATE compra 
            SET estado_compra = 'Paga',
                monto_total = @MontoFinal
            WHERE id_compra = @IdCompra",
            new { MontoFinal = montoFinal, IdCompra = idCompra }
        );
    }

    // Obtener una compra  por su id_compra
    public async Task<CompraResponseDTO?> ObtenerCompra(int idCompra)
    {
        using var conn = _db.CreateConnection();

        var compra = await conn.QueryFirstOrDefaultAsync<CompraResponseDTO>(@"
            SELECT id_compra, fecha_compra, estado_compra, monto_total, mail_comprador
            FROM compra
            WHERE id_compra = @IdCompra",
            new { IdCompra = idCompra }
        );

        if (compra == null) return null;

        var entradas = await conn.QueryAsync<EntradaResponseDTO>(@"
            SELECT id_entrada, costo_entrada, estado_entrada,
                   id_evento, nombre_sector, mail_titular
            FROM entrada
            WHERE id_compra = @IdCompra",
            new { IdCompra = idCompra }
        );

        compra.Entradas = entradas.ToList();
        return compra;
    }

    // Obtener las compras realizadas por un usuario_general a través de su mail
    public async Task<IEnumerable<CompraResponseDTO>> ObtenerComprasPorUsuario(string mail)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<CompraResponseDTO>(@"
            SELECT id_compra, fecha_compra, estado_compra, monto_total, mail_comprador
            FROM compra
            WHERE mail_comprador = @Mail",
            new { Mail = mail }
        );
    }
}
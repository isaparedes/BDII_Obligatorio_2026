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

    // Agregar una nueva entrada a una compra
    public async Task AgregarEntrada(int idCompra, EntradaItemDTO entrada)
    {
        using var conn = _db.CreateConnection();

        var idEstadio = await conn.QueryFirstOrDefaultAsync<int>(@"
            SELECT id_estadio FROM evento
            WHERE id_evento = @IdEvento",
            new { entrada.IdEvento }
        );

        var costoEntrada = await conn.QueryFirstOrDefaultAsync<decimal>(@"
            SELECT costo_sector FROM sector
            WHERE id_estadio = @IdEstadio AND nombre_sector = @NombreSector",
            new { idEstadio, entrada.NombreSector }
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
                IdEstadio = idEstadio,
                entrada.NombreSector
            }
        );
    }

    // Confirmar el pago de una compra
    public async Task ConfirmarCompra(int idCompra)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(@"
            UPDATE compra SET estado_compra = 'Paga'
            WHERE id_compra = @IdCompra",
            new { IdCompra = idCompra }
        );
    }

    // Obtener una compra por su id_compra
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

    // Obtener las compras de un usuario_general por su mail
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
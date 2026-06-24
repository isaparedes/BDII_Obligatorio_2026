using System.Data;
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
    public async Task<int> RealizarCompra(string mailComprador, List<EntradaItemDTO> entradas)
    {
        using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        using var tx = conn.BeginTransaction();

        try
        {
            var idComision = await conn.QueryFirstOrDefaultAsync<int>(
                @"SELECT id_comision
                  FROM comision
                  WHERE fecha_inicio <= CURDATE()
                    AND fecha_fin >= CURDATE()
                  LIMIT 1",
                transaction: tx);

            if (idComision == 0)
                throw new Exception("No hay comisión vigente");

            var idCompra = await conn.QueryFirstOrDefaultAsync<int>(
                @"INSERT INTO compra
                    (fecha_compra, estado_compra, monto_total, mail_comprador, id_comision)
                  VALUES
                    (CURDATE(), 'Pendiente', 1, @MailComprador, @IdComision);

                  SELECT LAST_INSERT_ID();",
                new
                {
                    MailComprador = mailComprador,
                    IdComision = idComision
                },
                tx);

            foreach (var entrada in entradas)
            {
                await AgregarEntrada(
                    conn,
                    tx,
                    idCompra,
                    entrada);
            }

            tx.Commit();

            return idCompra;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
    
    // Agregar una entrada a una compra
    private async Task AgregarEntrada(IDbConnection conn, IDbTransaction tx, int idCompra, EntradaItemDTO entrada)
    {
        var evento = await conn.QueryFirstOrDefaultAsync<Evento>(
            @"SELECT id_estadio,
                     fecha_evento,
                     hora_evento
              FROM evento
              WHERE id_evento = @IdEvento",
            new { entrada.IdEvento },
            tx);

        if (evento == null)
            throw new Exception("El evento no existe");

        DateTime fechaHoraEvento =
            evento.FechaEvento.Date.Add(evento.HoraEvento);

        if (fechaHoraEvento <= DateTime.Now)
            throw new Exception("No se pueden comprar entradas para un evento que ya ocurrió");

        var sectorHabilitado = await conn.QueryFirstOrDefaultAsync<int>(
            @"SELECT COUNT(*)
              FROM habilita
              WHERE id_evento = @IdEvento
                AND id_estadio = @IdEstadio
                AND nombre_sector = @NombreSector",
            new
            {
                entrada.IdEvento,
                evento.IdEstadio,
                entrada.NombreSector
            },
            tx);

        if (sectorHabilitado == 0)
            throw new Exception("El sector no está habilitado para este evento");

        var costoEntrada = await conn.QueryFirstOrDefaultAsync<decimal>(
            @"SELECT costo_sector
              FROM sector
              WHERE id_estadio = @IdEstadio
                AND nombre_sector = @NombreSector",
            new
            {
                evento.IdEstadio,
                entrada.NombreSector
            },
            tx);

        if (costoEntrada == 0)
            throw new Exception("Sector no encontrado");

        await conn.ExecuteAsync(@"
            INSERT INTO entrada
            (costo_entrada, estado_entrada, id_compra, id_evento, id_estadio, nombre_sector, mail_titular)
            VALUES
            (@CostoEntrada, 'Emitida', @IdCompra, @IdEvento, @IdEstadio, @NombreSector,'')",
            new
            {
                CostoEntrada = costoEntrada,
                IdCompra = idCompra,
                entrada.IdEvento,
                evento.IdEstadio,
                entrada.NombreSector
            },
            tx);
    }

    // Confirmar "pago" de compra
    public async Task PagarCompra(int idCompra)
    {
        using var conn = _db.CreateConnection();

        // Obtener suma de entradas y valor de comision

        var resultado = await conn.QueryFirstOrDefaultAsync<CompraComisionDto>(
                @"SELECT c.monto_total AS Subtotal, co.valor_comision AS ValorComision
                FROM compra c
                JOIN comision co ON c.id_comision = co.id_comision
                WHERE c.id_compra = @IdCompra",
                new { IdCompra = idCompra }
            );

        decimal montoFinal = resultado.Subtotal * (1 + resultado.ValorComision / 100m);

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
            SELECT id_entrada, costo_entrada, estado_entrada, id_evento, nombre_sector, mail_titular
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
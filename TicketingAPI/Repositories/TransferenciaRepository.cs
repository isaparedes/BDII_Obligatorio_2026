using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class TransferenciaRepository
{
    private readonly DatabaseConnection _db;

    public TransferenciaRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Crear nueva transferencia
    public async Task<int> CrearTransferencia(CrearTransferenciaDTO dto, string mailRemitente)
    {
        using var conn = _db.CreateConnection();

        var existeTransferencia = await conn.QueryFirstOrDefaultAsync<int>(@"
            SELECT 1
            FROM transferencia 
            WHERE id_entrada = @IdEntrada
            AND estado_transferencia = 'En proceso'
            LIMIT 1",
            new 
            { dto.IdEntrada }
        );

        var entrada = await conn.QueryFirstOrDefaultAsync<Entrada>(
            "SELECT * FROM entrada WHERE id_entrada = @IdEntrada",
            new { dto.IdEntrada }
        );

        if (entrada == null)
            throw new Exception("La entrada no existe");

        if (entrada.MailTitular != mailRemitente)
            throw new Exception("No eres el dueño de la entrada");


        if (existeTransferencia != 0)
            throw new Exception("Ya existe una transferencia en proceso con dicha entrada");
        

        var existeUsuario = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM usuario WHERE mail = @Mail",
            new { Mail = dto.MailDestinatario }
        );

        if (existeUsuario == 0)
            throw new Exception("El destinatario no existe");

        return await conn.QueryFirstOrDefaultAsync<int>(@"
            INSERT INTO transferencia 
            (fecha_transferencia, estado_transferencia, id_entrada, mail_remitente, mail_destinatario)
            VALUES 
            (CURDATE(), 'En proceso', @IdEntrada, @MailRemitente, @MailDestinatario);
            SELECT LAST_INSERT_ID();",
            new
            {
                dto.IdEntrada,
                MailRemitente = mailRemitente,
                dto.MailDestinatario
            }
        );
    }

    // Aceptar o rechazar transferencia de entrada
    public async Task ResponderTransferencia(int idTransferencia, string estadoTransferencia)
    {
        using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        using var tx = conn.BeginTransaction();

        try
        {
            var transferencia = await conn.QueryFirstOrDefaultAsync<Transferencia>(@"
                SELECT estado_transferencia, id_entrada, mail_destinatario
                FROM transferencia
                WHERE id_transferencia = @IdTransferencia
                FOR UPDATE",
                new { IdTransferencia = idTransferencia },
                tx
            );

            if (transferencia == null)
                throw new Exception("La transferencia no existe");

            if (transferencia.EstadoTransferencia != "En proceso")
                throw new Exception("La transferencia ya fue procesada");

            if (estadoTransferencia == "Aceptada")
            {
                await conn.ExecuteAsync(@"
                    UPDATE entrada
                    SET mail_titular = @MailDestinatario
                    WHERE id_entrada = @IdEntrada",
                    new
                    {
                        transferencia.IdEntrada,
                        transferencia.MailDestinatario
                    },
                    tx
                );

                await conn.ExecuteAsync(@"
                    UPDATE transferencia 
                    SET estado_transferencia = 'Aceptada',
                    fecha_aceptacion = NOW()
                    WHERE id_transferencia = @IdTransferencia",
                    new { IdTransferencia = idTransferencia },
                    tx
                );
            }
            else
            {
                await conn.ExecuteAsync(@"
                    UPDATE transferencia 
                    SET estado_transferencia = 'Rechazada'
                    WHERE id_transferencia = @IdTransferencia",
                    new { IdTransferencia = idTransferencia },
                    tx
                );
            }

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // Obtener todas las transferencias en las que participó cierta entrada por su id_entrada
    public async Task<IEnumerable<TransferenciaResponseDTO>> ObtenerPorEntrada(int idEntrada)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<TransferenciaResponseDTO>(
            "SELECT * FROM transferencia WHERE id_entrada = @IdEntrada",
            new { IdEntrada = idEntrada}
        );
    }

    // Obtener todas las transferencias enviadas por un mail_remitente
    public async Task<IEnumerable<TransferenciaResponseDTO>> ObtenerEnviadasPorUsuario(string mailRemitente)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<TransferenciaResponseDTO>(
            "SELECT * FROM transferencia WHERE mail_remitente = @MailRemitente",
            new { MailRemitente = mailRemitente}
        );
    }

    // Obtener todas las transferencias recibidas por un mail_destinatario
    public async Task<IEnumerable<TransferenciaResponseDTO>> ObtenerRecibidasPorUsuario(string mailDestinatario)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<TransferenciaResponseDTO>(
            "SELECT * FROM transferencia WHERE mail_destinatario = @MailDestinatario",
            new { MailDestinatario = mailDestinatario}
        );
    }

}
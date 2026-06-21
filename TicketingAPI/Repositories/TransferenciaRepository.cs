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

        if (estadoTransferencia == "Aceptada")
        {
            await conn.ExecuteAsync(@"
                UPDATE transferencia 
                SET estado_transferencia = @Estado,
                    fecha_aceptacion = CURDATE()
                WHERE id_transferencia = @IdTransferencia",
                new { Estado = estadoTransferencia, IdTransferencia = idTransferencia }
            );
        }
        else
        {
            await conn.ExecuteAsync(@"
                UPDATE transferencia 
                SET estado_transferencia = @Estado
                WHERE id_transferencia = @IdTransferencia",
                new { Estado = estadoTransferencia, IdTransferencia = idTransferencia }
            );
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
using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class EstadisticasRepository
{
    private readonly DatabaseConnection _db;

    public EstadisticasRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Obtener los usuarios_generales que son los mayores compradores (Top 10)
    public async Task<IEnumerable<MayorCompradorDTO>> ObtenerMayoresCompradores()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<MayorCompradorDTO>(@"
            SELECT c.mail_comprador, 
            COUNT(c.id_compra) AS total_compras, 
            COUNT(e.id_entrada) AS total_entradas, 
            SUM(c.monto_total) AS gasto_total
            FROM compra c
            JOIN entrada e
            ON c.id_compra = e.id_compra
            GROUP BY c.mail_comprador
            LIMIT 10"
        );
    }

    // Obtener los eventos que vendieron más entradas (Top 5)
    public async Task<IEnumerable<EventoMasVendidoDTO>> ObtenerEventosMasVendidos()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<EventoMasVendidoDTO>(@"
            SELECT ev.id_evento, 
            ev.equipo_local, ev.equipo_visitante,
            es.nombre_estadio,
            ev.fecha_evento,
            COUNT(en.id_entrada) AS total_entradas_vendidas
            FROM evento ev
            JOIN entrada en
            ON ev.id_evento = en.id_evento
            JOIN estadio es
            ON ev.id_estadio = es.id_estadio
            GROUP BY ev.id_evento
            LIMIT 5"
        );
    }
}
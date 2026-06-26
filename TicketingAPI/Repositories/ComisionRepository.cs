using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class ComisionRepository
{
    private readonly DatabaseConnection _db;

    public ComisionRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Crear nueva comision
    public async Task CrearComision(CrearComisionDTO dto)
    {
        using var conn = _db.CreateConnection();

        await conn.ExecuteAsync(@"
            INSERT INTO comision 
            (valor_comision, fecha_inicio, fecha_fin)
            VALUES
            (@ValorComision, @FechaInicio, @FechaFin)",
            new
            {
                dto.ValorComision,
                dto.FechaInicio,
                dto.FechaFin
            }
        );
    }
}
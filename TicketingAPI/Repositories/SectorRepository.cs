using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class SectorRepository
{
    private readonly DatabaseConnection _db;

    public SectorRepository(DatabaseConnection db)
    {
        _db = db;
    }

     public async Task<bool> ExisteSector(int idEstadio, string nombreSector)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM sector WHERE id_estadio = @IdEstadio AND nombre_sector = @NombreSector",
            new { 
                IdEstadio = idEstadio,
                NombreSector = nombreSector
            }
        );
        return resultado > 0;
    }

    public async Task CrearSector(CrearSectorDTO dto)
    {
        using var conn = _db.CreateConnection();

        await conn.ExecuteAsync(@"
            INSERT INTO sector 
            (id_estadio, nombre_sector, costo_sector, capacidad)
            VALUES
            (@IdEstadio, @NombreSector, @CostoSector, @Capacidad)",
            new
            {
                dto.IdEstadio,
                dto.NombreSector,
                dto.CostoSector,
                dto.Capacidad
            }
        );
    }
}
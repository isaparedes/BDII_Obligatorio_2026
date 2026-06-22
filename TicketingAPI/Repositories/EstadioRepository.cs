using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class EstadioRepository
{
    private readonly DatabaseConnection _db;

    public EstadioRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Averiguar si existe un estadio (por su nombre_estadio)
    public async Task<bool> ExisteEstadio(string nombreEstadio)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM estadio WHERE nombre_estadio = @NombreEstadio",
            new { NombreEstadio = nombreEstadio }
        );
        return resultado > 0;
    }

    // Crear nuevo estadio
    public async Task CrearEstadio(CrearEstadioDTO dto)
    {
        using var conn = _db.CreateConnection();

        await conn.ExecuteAsync(@"
            INSERT INTO estadio 
            (nombre_estadio, pais_estadio, ciudad_estadio, calle_estadio, numero_estadio)
            VALUES
            (@NombreEstadio, @PaisEstadio, @CiudadEstadio, @CalleEstadio, @NumeroEstadio)",
            new
            {
                dto.NombreEstadio,
                dto.PaisEstadio,
                dto.CiudadEstadio,
                dto.CalleEstadio,
                dto.NumeroEstadio,
            }
        );
    }

    // Obtener todos los estadios registrados
    public async Task<IEnumerable<Estadio>> ObtenerTodos()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Estadio>(
            "SELECT * FROM estadio"
        );
    }  

    // Obtener un estadio por su id_estadio
    public async Task<Estadio?> ObtenerPorId(int idEstadio)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Estadio>(
            "SELECT * FROM estadio WHERE id_estadio = @IdEstadio",
            new { IdEstadio = idEstadio }
        );
    }
}
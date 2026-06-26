using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class EquipoRepository
{
    private readonly DatabaseConnection _db;

    public EquipoRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Averiguar si existe un equipo (por su nombre_equipo)
     public async Task<bool> ExisteEquipo(string nombreEquipo)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM equipo WHERE nombre_equipo = @NombreEquipo",
            new { NombreEquipo = nombreEquipo }
        );
        return resultado > 0;
    }

    // Crear un nuevo equipo
    public async Task CrearEquipo(CrearEquipoDTO dto)
    {
        using var conn = _db.CreateConnection();

        await conn.ExecuteAsync(@"
            INSERT INTO equipo 
            (nombre_equipo)
            VALUES
            (@NombreEquipo)",
            new
            {
                dto.NombreEquipo
            }
        );
    }

    // Obtener todos los equipos registrados
    public async Task<IEnumerable<Equipo>> ObtenerTodos()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Equipo>(
            "SELECT * FROM equipo"
        );
    }
}
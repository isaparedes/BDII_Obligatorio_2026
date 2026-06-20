using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class EventoRepository
{
    private readonly DatabaseConnection _db;

    public EventoRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Averiguar si un evento en cierta fecha, hora y estadio ya fue creado
    public async Task<bool> ExisteEvento(DateTime fechaEvento, TimeSpan horaEvento, int idEstadio)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM evento WHERE fecha_evento = @FechaEvento AND hora_evento = @HoraEvento AND id_estadio = @IdEstadio",
            new { 
                FechaEvento = fechaEvento.Date,
                HoraEvento = horaEvento,
                IdEstadio = idEstadio
            }
        );
        return resultado > 0;
    }
    
    // Crear un nuevo evento
    public async Task CrearEvento(CrearEventoDTO dto, string mailAdmin)
    {
        using var conn = _db.CreateConnection();

        await conn.ExecuteAsync(@"
            INSERT INTO evento 
            (fecha_evento, hora_evento, id_estadio, equipo_local, equipo_visitante, mail_admin)
            VALUES
            (@FechaEvento, @HoraEvento, @IdEstadio, @EquipoLocal, @EquipoVisitante, @MailAdmin)",
            new
            {
                dto.FechaEvento,
                dto.HoraEvento,
                dto.IdEstadio,
                dto.EquipoLocal,
                dto.EquipoVisitante,
                MailAdmin = mailAdmin
            }
        );
    }

    // Obtener todos los eventos registrados
    public async Task<IEnumerable<Evento>> ObtenerTodos()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Evento>(
            "SELECT * FROM evento"
        );
    }  

    // Obtener un evento por su id_evento
    public async Task<Evento?> ObtenerPorId(int idEvento)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Evento>(
            "SELECT * FROM evento WHERE id_evento = @IdEvento",
            new { IdEvento = idEvento }
        );
    }

    // Averiguar si un sector ya fue habilitado para cierto evento
    public async Task<bool> ExisteHabilitacion(int idEvento, string nombreSector)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM habilita WHERE id_evento = @IdEvento AND nombre_sector = @NombreSector",
            new { 
                IdEvento = idEvento,
                NombreSector = nombreSector
            }
        );
        return resultado > 0;
    }

    // Habilitar un sector de un estadio para un evento
    public async Task HabilitarSector(HabilitarSectorDTO dto)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            @"INSERT INTO habilita
            (id_evento, id_estadio, nombre_sector)
            SELECT
                @IdEvento,
                e.id_estadio,
                @NombreSector
            FROM evento e
            WHERE e.id_evento = @IdEvento",
            new
            {
                dto.IdEvento,
                dto.NombreSector
            }
        );
    }

    // Obtener todos los sectores habilitados para un evento
    public async Task<IEnumerable<Habilita>> ObtenerSectoresHabilitados(int idEvento)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Habilita>(
            "SELECT nombre_sector FROM habilita WHERE id_evento = @IdEvento",
            new { IdEvento = idEvento }
        );
    }

    // Averiguar si un funcionario ya fue asignado a un sector para cierto evento
    public async Task<bool> ExisteAsignacion(int idEvento, string nombreSector, string mailFuncionario)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM asignacion WHERE id_evento = @IdEvento AND nombre_sector = @NombreSector AND mail_funcionario = @MailFuncionario",
            new { 
                IdEvento = idEvento,
                NombreSector = nombreSector,
                MailFuncionario = mailFuncionario
            }
        );
        return resultado > 0;
    }

    // Asignar un funcionario a un sector para cierto evento
    public async Task AsignarFuncionario(AsignarFuncionarioDTO dto)
    {
        using var conn = _db.CreateConnection();
        await conn.ExecuteAsync(
            @"INSERT INTO asignacion
            (id_evento, id_estadio, nombre_sector, mail_funcionario)
            SELECT
                @IdEvento,
                e.id_estadio,
                @NombreSector,
                @MailFuncionario
            FROM evento e
            WHERE e.id_evento = @IdEvento",
            new
            {
                dto.IdEvento,
                dto.NombreSector,
                dto.MailFuncionario
            }
        );
    }

    // Obtener todos los funcionarios asignados a sectores en un evento
    public async Task<IEnumerable<Asignacion>> ObtenerFuncionariosAsignados(int idEvento)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Asignacion>(
            "SELECT nombre_sector, mail_funcionario FROM asignacion WHERE id_evento = @IdEvento",
            new { IdEvento = idEvento }
        );   
    }
}
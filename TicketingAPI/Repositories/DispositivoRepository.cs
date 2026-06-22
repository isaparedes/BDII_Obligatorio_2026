using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class DispositivoRepository
{
    private readonly DatabaseConnection _db;

    public DispositivoRepository(DatabaseConnection db)
    {
        _db = db;
    }

    // Crear un nuevo dispositivo
    public async Task CrearDispositivo(CrearDispositivoDTO dto)
    {
        using var conn = _db.CreateConnection();

        var existeFuncionario = await conn.QueryFirstOrDefaultAsync<int>(@"
            SELECT COUNT(*) 
            FROM funcionario
            WHERE mail = @Mail",
            new { Mail = dto.MailFuncionario }
        );

        if (existeFuncionario == 0)
            throw new Exception("El funcionario no existe");

        await conn.ExecuteAsync(@"
            INSERT INTO dispositivo 
            (mail_funcionario)
            VALUES
            (@MailFuncionario)",
            new { dto.MailFuncionario }
        );
}

    // Obtener todos los dispositivos registrados
    public async Task<IEnumerable<Dispositivo>> ObtenerTodos()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Dispositivo>(
            "SELECT * FROM dispositivo"
        );
    }
}
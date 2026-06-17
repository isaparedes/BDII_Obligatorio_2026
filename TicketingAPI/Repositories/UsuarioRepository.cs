using Dapper;
using System.Data;
using TicketingAPI.Database;
using TicketingAPI.DTOs;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories;

public class UsuarioRepository
{
    private readonly DatabaseConnection _db;

    public UsuarioRepository(DatabaseConnection db)
    {
        _db = db;
    }

    public async Task<bool> ExisteMail(string mail)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM usuario WHERE mail = @Mail",
            new { Mail = mail }
        );
        return resultado > 0;
    }

    private async Task InsertarUsuarioBase(IDbConnection conn, string mail, string hash, dynamic dto)
    {
        await conn.ExecuteAsync(@"
            INSERT INTO usuario 
            (mail, contrasena, pais_documento, tipo_documento, numero_documento,
             pais_direccion, localidad, calle, numero_calle, codigo_postal)
            VALUES 
            (@Mail, @Contrasena, @PaisDocumento, @TipoDocumento, @NumeroDocumento,
             @PaisDireccion, @Localidad, @Calle, @NumeroCalle, @CodigoPostal)",
            new
            {
                Mail = mail,
                Contrasena = hash,
                dto.PaisDocumento,
                dto.TipoDocumento,
                dto.NumeroDocumento,
                dto.PaisDireccion,
                dto.Localidad,
                dto.Calle,
                dto.NumeroCalle,
                dto.CodigoPostal
            }
        );

        foreach (var tel in dto.Telefonos)
        {
            await conn.ExecuteAsync(@"
                INSERT INTO usuario_telefono (mail_usuario, numero_telefono)
                VALUES (@Mail, @Telefono)",
                new { Mail = mail, Telefono = tel }
            );
        }
    }

    public async Task RegistrarUsuarioGeneral(RegistroUsuarioDTO dto, string hash)
    {
        using var conn = _db.CreateConnection();
        await InsertarUsuarioBase(conn, dto.Mail, hash, dto);
        await conn.ExecuteAsync(
            "INSERT INTO usuario_general (mail, fecha_registro) VALUES (@Mail, CURDATE())",
            new { dto.Mail }
        );
    }

    public async Task RegistrarAdministrador(RegistroAdministradorDTO dto, string hash)
    {
        using var conn = _db.CreateConnection();
        await InsertarUsuarioBase(conn, dto.Mail, hash, dto);
        await conn.ExecuteAsync(
            "INSERT INTO administrador (mail, fecha_asignacion, pais_sede) VALUES (@Mail, @FechaAsignacion, @PaisSede)",
            new { dto.Mail, dto.FechaAsignacion, dto.PaisSede }
        );
    }

    public async Task RegistrarFuncionario(RegistroFuncionarioDTO dto, string hash)
    {
        using var conn = _db.CreateConnection();
        await InsertarUsuarioBase(conn, dto.Mail, hash, dto);
        await conn.ExecuteAsync(
            "INSERT INTO funcionario (mail, numero_legajo) VALUES (@Mail, @NumeroLegajo)",
            new { dto.Mail, dto.NumeroLegajo }
        );
    }

    public async Task<Usuario?> ObtenerPorMail(string mail)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>(
            "SELECT * FROM usuario WHERE mail = @Mail",
            new { Mail = mail }
        );
    }

    public async Task<string?> ObtenerRol(string mail)
    {
        using var conn = _db.CreateConnection();

        var esAdmin = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM administrador WHERE mail = @Mail",
            new { Mail = mail }
        );
        if (esAdmin > 0) return "Administrador";

        var esFuncionario = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM funcionario WHERE mail = @Mail",
            new { Mail = mail }
        );
        if (esFuncionario > 0) return "Funcionario";

        var esGeneral = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM usuario_general WHERE mail = @Mail",
            new { Mail = mail }
        );
        if (esGeneral > 0) return "UsuarioGeneral";

        return null;
    }

    public async Task<bool> ValidarCredenciales(string mail, string hashContrasena)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM usuario WHERE mail = @Mail AND contrasena = @Contrasena",
            new { Mail = mail, Contrasena = hashContrasena }
        );
        return resultado > 0;
    }
}
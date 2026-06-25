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

    // Averiguar si existe un usuario (por su mail)
    public async Task<bool> ExisteMail(string mail)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM usuario WHERE mail = @Mail",
            new { Mail = mail }
        );
        return resultado > 0;
    }
    
    // Registrar nuevo usuario
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

    // Registrar datos de un nuevo usuario_general
    public async Task RegistrarUsuarioGeneral(RegistroUsuarioDTO dto, string hash)
    {
        using var conn = _db.CreateConnection();
        await InsertarUsuarioBase(conn, dto.Mail, hash, dto);
        await conn.ExecuteAsync(
            "INSERT INTO usuario_general (mail, fecha_registro) VALUES (@Mail, CURDATE())",
            new { dto.Mail }
        );
    }

    // Registrar datos de un nuevo administrador

    public async Task RegistrarAdministrador(RegistroAdministradorDTO dto, string hash)
    {
        using var conn = _db.CreateConnection();
        await InsertarUsuarioBase(conn, dto.Mail, hash, dto);
        await conn.ExecuteAsync(
            "INSERT INTO administrador (mail, fecha_asignacion, pais_sede) VALUES (@Mail, CURDATE(), @PaisSede)",
            new { dto.Mail, dto.PaisSede }
        );
    }

    // Registrar datos de un nuevo funcionario
    public async Task RegistrarFuncionario(RegistroFuncionarioDTO dto, string hash)
    {
        using var conn = _db.CreateConnection();
        await InsertarUsuarioBase(conn, dto.Mail, hash, dto);
        await conn.ExecuteAsync(
            "INSERT INTO funcionario (mail, numero_legajo) VALUES (@Mail, @NumeroLegajo)",
            new { dto.Mail, dto.NumeroLegajo }
        );
    }

    // Agregar a un funcionario/administrador un registro de su usuario en usuario_general para que pueda realizar las acciones de este rol también
    public async Task AgregarRolGeneral(string mail)
    {
        using var conn = _db.CreateConnection();

        var existeUsuarioGeneral = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM usuario_general WHERE mail = @Mail",
            new { Mail = mail }
        );

        if (existeUsuarioGeneral > 0)
            throw new Exception("El usuario ya tiene rol de usuario general");

        await conn.ExecuteAsync(
            "INSERT INTO usuario_general (mail, estado_verificacion, fecha_registro) VALUES (@Mail, 'Aprobado', CURDATE())",
            new { Mail = mail }
        );
    }

    // Obtener un usuario por su mail
    public async Task<Usuario?> ObtenerPorMail(string mail)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>(
            "SELECT * FROM usuario WHERE mail = @Mail",
            new { Mail = mail }
        );
    }

    // Obtener el rol de un usuario por su mail
    public async Task<List<string>> ObtenerRoles(string mail)
    {
        using var conn = _db.CreateConnection();
        var roles = new List<string>();

        var esAdmin = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM administrador WHERE mail = @Mail", new { Mail = mail });
        if (esAdmin > 0) roles.Add("Administrador");

        var esFuncionario = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM funcionario WHERE mail = @Mail", new { Mail = mail });
        if (esFuncionario > 0) roles.Add("Funcionario");

        var esGeneral = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM usuario_general WHERE mail = @Mail", new { Mail = mail });
        if (esGeneral > 0) roles.Add("UsuarioGeneral");

        return roles;
    }

    // Validar inicio de sesión de un usuario
    public async Task<bool> ValidarCredenciales(string mail, string hashContrasena)
    {
        using var conn = _db.CreateConnection();
        var resultado = await conn.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM usuario WHERE mail = @Mail AND contrasena = @Contrasena",
            new { Mail = mail, Contrasena = hashContrasena }
        );
        return resultado > 0;
    }

    // Cambiar estado de verificación de un usuario (aprobar o rechazar)
    public async Task<int> VerificarUsuarioGral(VerificarUsuarioGralDTO dto)
    {
        using var conn = _db.CreateConnection();

        return await conn.ExecuteAsync(
            @"UPDATE usuario_general
            SET estado_verificacion = @EstadoVerificacion
            WHERE mail = @Mail",
            new
            {
                dto.EstadoVerificacion,
                dto.Mail
            }
        );
    }

}
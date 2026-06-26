using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using TicketingAPI.Services;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioRepository _repo;
    private readonly JwtService _jwtService;

    public UsuarioController(UsuarioRepository repo, JwtService jwtService)
    {
        _repo = repo;
        _jwtService = jwtService;
    }

    private string HashContrasena(string contrasena) =>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(contrasena))
        ).ToLower();

    [HttpPost("registro/usuario-general")]
    [AllowAnonymous]
    public async Task<IActionResult> RegistrarUsuarioGeneral([FromBody] RegistroUsuarioDTO dto)
    {
        if (await _repo.ExisteMail(dto.Mail))
            return BadRequest("El mail ya está registrado");

        await _repo.RegistrarUsuarioGeneral(dto, HashContrasena(dto.Contrasena));
        return Ok("Usuario general registrado correctamente");
    }

    [HttpPost("registro/administrador")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegistrarAdministrador([FromBody] RegistroAdministradorDTO dto)
    {
        if (await _repo.ExisteMail(dto.Mail))
            return BadRequest("El mail ya está registrado");

        await _repo.RegistrarAdministrador(dto, HashContrasena(dto.Contrasena));
        return Ok("Administrador registrado correctamente");
    }

    [HttpPost("registro/funcionario")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegistrarFuncionario([FromBody] RegistroFuncionarioDTO dto)
    {
        if (await _repo.ExisteMail(dto.Mail))
            return BadRequest("El mail ya está registrado");

        await _repo.RegistrarFuncionario(dto, HashContrasena(dto.Contrasena));
        return Ok("Funcionario registrado correctamente");
    }

    [HttpPost("agregar-rol-general")]
    [Authorize(Roles = "Funcionario, Administrador")]
    public async Task<IActionResult> AgregarRolGeneral()
    {
        var mail = User.FindFirst(ClaimTypes.Email)?.Value!;
        try
        {
            await _repo.AgregarRolGeneral(mail);
            return Ok("Rol de usuario general agregado correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var credencialesValidas = await _repo.ValidarCredenciales(dto.Mail, HashContrasena(dto.Contrasena));
        if (!credencialesValidas)
            return Unauthorized("Mail o contraseña incorrectos");

        var roles = await _repo.ObtenerRoles(dto.Mail);
        if (roles == null)
            return Unauthorized("Usuario sin rol asignado");

        var token = _jwtService.GenerarToken(dto.Mail, roles);

        return Ok(new LoginResponseDTO
        {
            Token = token,
            Mail = dto.Mail,
            Roles = roles
        });
    }

    // Chequear esto
    [HttpGet("{mail}")]
    [Authorize]
    public async Task<IActionResult> ObtenerUsuario(string mail)
    {
        var usuario = await _repo.ObtenerPorMail(mail);
        if (usuario == null)
            return NotFound("Usuario no encontrado");

        return Ok(usuario);
    }

    [HttpGet("{mail}/funcionario")]
    [Authorize]
    public async Task<IActionResult> ObtenerFuncionario(string mail)
    {
        var funcionario = await _repo.ObtenerFuncionario(mail);
        if (funcionario == null)
            return NotFound("Funcionario no encontrado");

        return Ok(funcionario);
    }

    [HttpPut("verificar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> VerificarUsuario([FromBody] VerificarUsuarioGralDTO dto)
    {
        if (dto.EstadoVerificacion != "Aprobado" &&
            dto.EstadoVerificacion != "No aprobado")
        {
            return BadRequest(
                "El estado debe ser 'Aprobado' o 'No aprobado'"
            );
        }

        var filas = await _repo.VerificarUsuarioGral(dto);

        if (filas == 0)
            return NotFound("Usuario general no encontrado");

        return Ok("Estado de verificación actualizado correctamente");
    }


}
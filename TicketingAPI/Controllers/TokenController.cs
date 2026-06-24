using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class TokenController : ControllerBase
{
    private readonly TokenRepository _tokenRepo;
    private readonly EntradaRepository _entradaRepo;

    public TokenController(TokenRepository tokenRepo, EntradaRepository entradaRepo)
    {
        _tokenRepo = tokenRepo;
        _entradaRepo = entradaRepo;

    }


    [HttpGet("activo/{idEntrada}")]
    [Authorize(Roles = "UsuarioGeneral")]
    public async Task<IActionResult> ObtenerTokenActivo(int idEntrada)
    {
        // Verificar que la entrada pertenece al usuario
        var mail = User.FindFirst(ClaimTypes.Email)?.Value!;
        var entrada = await _entradaRepo.ObtenerPorId(idEntrada);

        if (entrada == null)
            return NotFound("Entrada no encontrada");

        if (entrada.MailTitular != mail)
            return Forbid();

        if (entrada.EstadoEntrada == "Consumida")
            return BadRequest("La entrada ya fue consumida");

        try
        {
            var token = await _tokenRepo.ObtenerOGenerarTokenActivo(idEntrada);
            return Ok(new TokenResponseDTO
            {
                CodigoQR = token.CodigoQr,
                FechaHoraExpiracion = token.FechaHoraExpiracion!.Value,
                EstadoToken = token.EstadoToken
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
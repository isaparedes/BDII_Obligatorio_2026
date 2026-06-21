using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntradaController : ControllerBase
{
    private readonly EntradaRepository _repo;

    public EntradaController(EntradaRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("mis-entradas")]
    [Authorize(Roles = "UsuarioGeneral")] 
    public async Task<IActionResult> ObtenerMisEntradas()
    {
        var mail = User.FindFirst(ClaimTypes.Email)?.Value!;
        var entradas = await _repo.ObtenerPorTitular(mail);
        return Ok(entradas);
    }

    [HttpGet("{idEntrada}")]
    [Authorize(Roles = "UsuarioGeneral")] 
    public async Task<IActionResult> ObtenerEntrada(int idEntrada)
    {
        var entrada = await _repo.ObtenerPorId(idEntrada);
        if (entrada == null)
            return NotFound("Entrada no encontrada");
        return Ok(entrada);
    }
}
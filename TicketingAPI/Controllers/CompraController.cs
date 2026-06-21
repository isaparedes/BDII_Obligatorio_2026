using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompraController : ControllerBase
{
    private readonly CompraRepository _repo;

    public CompraController(CompraRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    [Authorize(Roles = "UsuarioGeneral")]
    public async Task<IActionResult> RealizarCompra([FromBody] CrearCompraDTO dto)
    {
        if (dto.Entradas == null || dto.Entradas.Count == 0)
            return BadRequest("Debe incluir al menos una entrada");

        if (dto.Entradas.Count > 5)
            return BadRequest("No puede comprar más de 5 entradas por transacción");

        var mailComprador = User.FindFirst(ClaimTypes.Email)?.Value!;

        try
        {
            var idCompra = await _repo.CrearCompra(mailComprador);

            foreach (var entrada in dto.Entradas)
            {
                await _repo.AgregarEntrada(idCompra, entrada);
            }

            await _repo.ConfirmarCompra(idCompra);

            var compra = await _repo.ObtenerCompra(idCompra);
            return Ok(compra);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{idCompra}")]
    [Authorize]
    public async Task<IActionResult> ObtenerCompra(int idCompra)
    {
        var compra = await _repo.ObtenerCompra(idCompra);
        if (compra == null)
            return NotFound("Compra no encontrada");
        return Ok(compra);
    }

    [HttpGet("mis-compras")]
    [Authorize(Roles = "UsuarioGeneral")]
    public async Task<IActionResult> ObtenerMisCompras()
    {
        var mail = User.FindFirst(ClaimTypes.Email)?.Value!;
        var compras = await _repo.ObtenerComprasPorUsuario(mail);
        return Ok(compras);
    }
}
using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadisticasController : ControllerBase
{
    private readonly EstadisticasRepository _repo;

    public EstadisticasController(EstadisticasRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("mayores-compradores")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerMayoresCompradores()
    {
        var compradores = await _repo.ObtenerMayoresCompradores();
        return Ok(compradores);
    }

    [HttpGet("eventos-mas-vendidos")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerEventosMasVendidos()
    {
        var eventos = await _repo.ObtenerEventosMasVendidos();
        return Ok(eventos);
    }
}
using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class EstadioController : ControllerBase
{
    private readonly EstadioRepository _repo;

    public EstadioController(EstadioRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearEstadio([FromBody] CrearEstadioDTO dto)
    {
         if (await _repo.ExisteEstadio(dto.NombreEstadio))
            return BadRequest("El estadio ya está registrado");
            
        await _repo.CrearEstadio(dto);
        return Ok("Estadio creado correctamente");
    }
}

using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class EquipoController : ControllerBase
{
    private readonly EquipoRepository _repo;

    public EquipoController(EquipoRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearEquipo([FromBody] CrearEquipoDTO dto)
    {
         if (await _repo.ExisteEquipo(dto.NombreEquipo))
            return BadRequest("El sector ya está registrado");
            
        await _repo.CrearEquipo(dto);
        return Ok("Sector creado correctamente");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ObtenerEquipos()
    {
        var equipos = await _repo.ObtenerTodos();
        return Ok(equipos);
    }

}

using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class SectorController : ControllerBase
{
    private readonly SectorRepository _repo;

    public SectorController(SectorRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearSector([FromBody] CrearSectorDTO dto)
    {
         if (await _repo.ExisteSector(dto.IdEstadio, dto.NombreSector))
            return BadRequest("El sector ya está registrado");
            
        await _repo.CrearSector(dto);
        return Ok("Sector creado correctamente");
    }

    [HttpGet("{idEstadio}")]
    [Authorize]
    public async Task<IActionResult> ObtenerPorEstadio(int idEstadio)
    {
        var sectores = await _repo.ObtenerPorEstadio(idEstadio);
        return Ok(sectores);
    }
}

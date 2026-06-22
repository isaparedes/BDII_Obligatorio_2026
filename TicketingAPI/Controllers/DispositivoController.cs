 using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class DispositivoController : ControllerBase
{
    private readonly DispositivoRepository _repo;

    public DispositivoController(DispositivoRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearDispositivo([FromBody] CrearDispositivoDTO dto)
    {
        await _repo.CrearDispositivo(dto);
        return Ok("Dispositivo creado correctamente");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ObtenerDispositivos()
    {
        var equipos = await _repo.ObtenerTodos();
        return Ok(equipos);
    }

}

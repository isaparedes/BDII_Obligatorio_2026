using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class EventoController : ControllerBase
{
    private readonly EventoRepository _repo;

    public EventoController(EventoRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearEvento([FromBody] CrearEventoDTO dto)
    {
        /*
        if (await _repo.ExisteEvento(dto.FechaEvento, dto.HoraEvento, dto.IdEstadio))
            return BadRequest("Un evento en ese estadio en esa fecha ya está registrado");
        */

        var mailAdmin = User.FindFirst(ClaimTypes.Email)?.Value;
        await _repo.CrearEvento(dto, mailAdmin!);
        return Ok("Evento creado correctamente");
    }

    [HttpPost("habilitar-sector")]
    [Authorize(Roles = "Administrador")]
     public async Task<IActionResult> HabilitarSector([FromBody] HabilitarSectorDTO dto)
    {
        if (await _repo.ExisteHabilitacion(dto.IdEvento, dto.NombreSector))
            return BadRequest("Ese sector ya está habilitado para dicho evento");

        await _repo.HabilitarSector(dto);
        return Ok("Sector habilitado correctamente");
    }

    [HttpPost("asignar-funcionario")]
    [Authorize(Roles = "Administrador")]
     public async Task<IActionResult> AsignarFuncionario([FromBody] AsignarFuncionarioDTO dto)
    {
        if (await _repo.ExisteAsignacion(dto.IdEvento, dto.NombreSector, dto.MailFuncionario))
            return BadRequest("Ese funcionario ya fue asignado a dicho sector");

        await _repo.AsignarFuncionario(dto);
        return Ok("Funcionario asignado correctamente");
    }
}

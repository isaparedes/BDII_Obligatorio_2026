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
        
        if (await _repo.ExisteEvento(dto.FechaEvento, dto.HoraEvento, dto.IdEstadio))
            return BadRequest("Un evento en ese estadio en esa fecha ya está registrado");
        
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

    [HttpGet("{idEvento}/sectores-habilitados")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerSectoresHabilitados(int idEvento)
    {
        var sectores = await _repo.ObtenerSectoresHabilitados(idEvento);

        if (sectores == null || !sectores.Any())
            return NotFound("No hay sectores habilitados para este evento");

        return Ok(sectores);
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

    [HttpGet("{idEvento}/funcionarios-asignados")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerFuncionariosAsignados(int idEvento)
    {
        var asignaciones = await _repo.ObtenerFuncionariosAsignados(idEvento);

        if (asignaciones == null || !asignaciones.Any())
            return NotFound("No hay funcionarios asignados a este evento");

        return Ok(asignaciones);
    }

    [HttpGet("{idEvento}/dispositivos-habilitados")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerDispositivosHabilitados(int idEvento)
    {
        var dispositivos = await _repo.ObtenerDispositivosHabilitados(idEvento);

        if (dispositivos == null || !dispositivos.Any())
            return NotFound("No hay dispositivos habilitados para este evento");

        return Ok(dispositivos);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")] 
    public async Task<IActionResult> ObtenerTodos()
    {
        var eventos = await _repo.ObtenerTodos();
        return Ok(eventos);
    }

    [HttpGet("futuros")]
    [Authorize]
    public async Task<IActionResult> ObtenerFuturos()
    {
        var eventos = await _repo.ObtenerFuturos();

        return Ok(eventos);
    }

    [HttpGet("{idEvento}")]
    [Authorize] 
    public async Task<IActionResult> ObtenerPorId(int idEvento)
    {
        var evento = await _repo.ObtenerPorId(idEvento);

        if (evento == null)
            return NotFound("Evento no encontrado");

        return Ok(evento);
    }

}

using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ComisionController : ControllerBase
{
    private readonly ComisionRepository _repo;

    public ComisionController(ComisionRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearComision([FromBody] CrearComisionDTO dto)
    {
        await _repo.CrearComision(dto);
        return Ok("Comisión creada correctamente");
    }
}
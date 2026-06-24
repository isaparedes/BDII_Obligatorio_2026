using Microsoft.AspNetCore.Mvc;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ValidacionController : ControllerBase
{
    private readonly ValidacionRepository _repo;

    public ValidacionController(ValidacionRepository repo)
    {
        _repo = repo;
    }


    [HttpPost("escanear")]
    [Authorize(Roles = "Funcionario")]
    public async Task<IActionResult> Escanear([FromBody] ValidarTokenDTO dto)
    {
        var resultado = await _repo.ValidarToken(dto);

        if (resultado == "Acceso permitido")
            return Ok(resultado);

        return BadRequest(resultado);
    }
}
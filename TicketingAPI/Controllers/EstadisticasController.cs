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

    /*
    [HttpGet]
    [Authorize(Roles = "Administrador")] 
    public async Task<IActionResult> ObtenerMayoresCompradores()
    {
        
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")] 
    public async Task<IActionResult> ObtenerEventosMasVendidos()
    {
        
    }
    */

} 
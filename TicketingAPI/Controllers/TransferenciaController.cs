using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TicketingAPI.DTOs;
using TicketingAPI.Repositories;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferenciaController : ControllerBase
{
    private readonly TransferenciaRepository _repo;

    public TransferenciaController(TransferenciaRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    [Authorize(Roles = "UsuarioGeneral")]
    public async Task<IActionResult> RealizarTransferencia([FromBody] CrearTransferenciaDTO dto)
    {
        if (dto.IdEntrada == 0)
            return BadRequest("Debe incluir una entrada para la transferencia");

        if (string.IsNullOrEmpty(dto.MailDestinatario))
            return BadRequest("Debe incluir un mail para el destinatario");

        var mailRemitente = User.FindFirst(ClaimTypes.Email)?.Value!;

        try
        {
            var idTransferencia = await _repo.CrearTransferencia(dto, mailRemitente);
            return Ok(new { IdTransferencia = idTransferencia, Mensaje = "Transferencia creada correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{idTransferencia}/responder")]
    [Authorize(Roles = "UsuarioGeneral")]
    public async Task<IActionResult> ResponderTransferencia(int idTransferencia, [FromBody] ResponderTransferenciaDTO dto)
    {
        if (dto.EstadoTransferencia != "Aceptada" && dto.EstadoTransferencia != "Rechazada")
            return BadRequest("El estado debe ser Aceptada o Rechazada");

        try
        {
            await _repo.ResponderTransferencia(idTransferencia, dto.EstadoTransferencia);
            return Ok($"Transferencia {dto.EstadoTransferencia.ToLower()} correctamente");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("mis-transferencias")]
    [Authorize(Roles = "UsuarioGeneral")]
    public async Task<IActionResult> ObtenerMisTransferencias()
    {
        var mail = User.FindFirst(ClaimTypes.Email)?.Value!;
        var enviadas = await _repo.ObtenerEnviadasPorUsuario(mail);
        var recibidas = await _repo.ObtenerRecibidasPorUsuario(mail);
        return Ok(new { Enviadas = enviadas, Recibidas = recibidas });
    }

}
namespace TicketingAPI.Models;

public class UsuarioGeneral
{
    public string Mail { get; set; } = string.Empty;
    public string EstadoVerificacion { get; set; } = "Pendiente";
    public DateOnly FechaRegistro { get; set; }
}
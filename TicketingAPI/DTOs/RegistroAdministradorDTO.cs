namespace TicketingAPI.DTOs;

public class RegistroAdministradorDTO : RegistroUsuarioDTO
{
    // ver si cambiamos para que sea automático o no
    public DateTime FechaAsignacion { get; set; }
    public string PaisSede { get; set; } = string.Empty;
}
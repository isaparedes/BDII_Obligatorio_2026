namespace TicketingAPI.DTOs;

public class RegistroAdministradorDTO : RegistroUsuarioDTO
{
    public DateTime FechaAsignacion { get; set; }
    public string PaisSede { get; set; } = string.Empty;
}
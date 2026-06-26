namespace TicketingAPI.DTOs;

public class RegistroAdministradorDTO : RegistroUsuarioDTO
{
    public string PaisSede { get; set; } = string.Empty;
}
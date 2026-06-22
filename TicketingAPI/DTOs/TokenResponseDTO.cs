namespace TicketingAPI.DTOs;

public class TokenResponseDTO
{
    public string CodigoQR { get; set; } = string.Empty;
    public DateTime FechaHoraExpiracion { get; set; }
    public string EstadoToken { get; set; } = string.Empty;

}

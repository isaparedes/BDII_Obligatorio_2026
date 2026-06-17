namespace TicketingAPI.Models;

public class Token
{
    public string CodigoQr { get; set; } = string.Empty;
    public string EstadoToken { get; set; } = string.Empty;
    public DateTime FechaHoraVigencia { get; set; }
    public DateTime? FechaHoraExpiracion { get; set; }
    public DateTime? FechaHoraValidacion { get; set; }
    public int IdEntrada { get; set; }
    public int? IdDispositivoValida { get; set; }
}
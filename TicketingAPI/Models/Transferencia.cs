namespace TicketingAPI.Models;

public class Transferencia
{
    public int IdTransferencia { get; set; }
    public DateTime FechaTransferencia { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public string EstadoTransferencia { get; set; } = string.Empty;
    public int IdEntrada { get; set; }
    public string MailRemitente { get; set; } = string.Empty;
    public string MailDestinatario { get; set; } = string.Empty;
}
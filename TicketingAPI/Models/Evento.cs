namespace TicketingAPI.Models;

public class Evento
{
    public int IdEvento { get; set; }
    public DateTime FechaEvento { get; set; }
    public TimeOnly HoraEvento { get; set; }
    public int IdEstadio { get; set; }
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
    public string MailAdmin { get; set; } = string.Empty;
}
namespace TicketingAPI.DTOs;

public class CrearEventoDTO
{
    public DateTime FechaEvento { get; set; }
    public TimeSpan HoraEvento { get; set; } 
    public int IdEstadio { get; set; }
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
}
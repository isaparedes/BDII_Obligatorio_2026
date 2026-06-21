namespace TicketingAPI.DTOs;

public class EventoResponseDTO
{
    public int IdEvento { get; set; }
    public DateTime FechaEvento { get; set; }
    public TimeSpan HoraEvento { get; set; } 
    public string NombreEstadio { get; set; } = string.Empty;
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
}
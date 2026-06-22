namespace TicketingAPI.DTOs;

public class EventoMasVendidoDTO
{
    public int IdEvento { get; set; }
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public string NombreEstadio { get; set; } = string.Empty;
    public int TotalEntradasVendidas { get; set; }
}


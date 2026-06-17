namespace TicketingAPI.Models;

public class Estadio
{
    public int IdEstadio { get; set; }
    public string NombreEstadio { get; set; } = string.Empty;
    public string PaisEstadio { get; set; } = string.Empty;
    public string CiudadEstadio { get; set; } = string.Empty;
    public string CalleEstadio { get; set; } = string.Empty;
    public int NumeroEstadio { get; set; }
}
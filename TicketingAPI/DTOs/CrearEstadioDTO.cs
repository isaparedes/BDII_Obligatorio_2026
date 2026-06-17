namespace TicketingAPI.DTOs;

public class CrearEstadioDTO
{
    public string NombreEstadio { get; set; } = string.Empty;
    public string PaisEstadio { get; set; } = string.Empty;
    public string CiudadEstadio { get; set; } = string.Empty;
    public string CalleEstadio { get; set; } = string.Empty;
    public int NumeroEstadio { get; set; }
}
namespace TicketingAPI.DTOs;

public class CrearSectorDTO
{
    public int IdEstadio { get; set; }
    public string NombreSector { get; set; } = string.Empty;
    public decimal CostoSector { get; set; }
    public int Capacidad { get; set; }
}
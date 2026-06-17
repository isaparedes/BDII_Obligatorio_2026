namespace TicketingAPI.Models;

public class Sector
{
    public int IdEstadio { get; set; }
    public string NombreSector { get; set; } = string.Empty;
    public int CostoSector { get; set; }
    public int Capacidad { get; set; }
}
namespace TicketingAPI.Models;

public class Administrador
{
    public string Mail { get; set; } = string.Empty;
    public DateOnly FechaAsignacion { get; set;}

    public string PaisSede { get; set; } = string.Empty;
}
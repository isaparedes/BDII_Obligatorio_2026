namespace TicketingAPI.Models;

public class Comision
{
    public int IdComision { get; set; }
    public decimal ValorComision { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
}
namespace TicketingAPI.Models;

public class Compra
{
    public int IdCompra { get; set; }
    public DateOnly FechaCompra { get; set; }
    public string EstadoCompra { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public string MailComprador { get; set; } = string.Empty;
    public int IdComision { get; set; }
}
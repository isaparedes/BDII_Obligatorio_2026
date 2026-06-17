namespace TicketingAPI.Models;

public class Entrada
{
    public int IdEntrada { get; set; }
    public decimal CostoEntrada { get; set; }
    public string EstadoEntrada { get; set; } = string.Empty;
    public int IdCompra { get; set; }
    public int IdEvento { get; set; }
    public int IdEstadio { get; set; }
    public string NombreSector { get; set; } = string.Empty;
    public string MailTitular { get; set; } = string.Empty;

}
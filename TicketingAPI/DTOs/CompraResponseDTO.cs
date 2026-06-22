namespace TicketingAPI.DTOs;

public class CompraResponseDTO
{
    public int IdCompra { get; set; }
    public string FechaCompra { get; set; } = string.Empty;
    public string EstadoCompra { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public string MailComprador { get; set; } = string.Empty;
    public List<EntradaResponseDTO> Entradas { get; set; } = new();
}
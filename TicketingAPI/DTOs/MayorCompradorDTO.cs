namespace TicketingAPI.DTOs;

public class MayorCompradorDTO
{
    public string MailComprador { get; set; } = string.Empty;
    public int TotalCompras { get; set; }
    public int TotalEntradas { get; set; }
    public decimal GastoTotal { get; set; }
}

namespace TicketingAPI.DTOs;

public class MayorCompradorDTO
{
    public string Mail { get; set; } = string.Empty;
    public int TotalCompras { get; set; }
    public int TotalEntradas { get; set; }
    public decimal GastoTotal { get; set; }
}

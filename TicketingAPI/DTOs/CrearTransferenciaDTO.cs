namespace TicketingAPI.DTOs;

public class CrearTransferenciaDTO
{
    public int IdEntrada { get; set; }
    public string MailDestinatario { get; set; } = string.Empty;
}
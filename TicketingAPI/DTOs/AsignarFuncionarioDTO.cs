namespace TicketingAPI.DTOs;

public class AsignarFuncionarioDTO
{
    public int IdEvento { get; set; }
    public string NombreSector{ get; set; } = string.Empty;
    public string MailFuncionario { get; set; } = string.Empty;
}
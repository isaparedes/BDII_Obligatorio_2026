namespace TicketingAPI.Models;

public class Asignacion
{
    public int IdEvento { get; set; }
    public int IdEstadio { get; set; }
    public string NombreSector { get; set; } = string.Empty;
    public string MailFuncionario { get; set; } = string.Empty;

}
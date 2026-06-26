namespace TicketingAPI.Models;

public class Usuario
{
    public string Mail { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string PaisDocumento { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string PaisDireccion { get; set; } = string.Empty;
    public string Localidad { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public string NumeroCalle { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
}
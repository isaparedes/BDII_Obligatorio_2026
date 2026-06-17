namespace TicketingAPI.DTOs;

public class RegistroUsuarioDTO
{
    public string Mail { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string PaisDocumento { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string PaisDireccion { get; set; } = string.Empty;
    public string Localidad { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public int NumeroCalle { get; set; }
    public string CodigoPostal { get; set; } = string.Empty;
    public List<string> Telefonos { get; set; } = new();
}
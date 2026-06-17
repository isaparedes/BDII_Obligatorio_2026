namespace TicketingAPI.DTOs;

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
namespace TicketingAPI.DTOs;

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TicketingAPI.Services;

public class JwtService
{
    private readonly string _secret;
    private readonly int _expirationHours;

    public JwtService(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"]!;
        _expirationHours = int.Parse(configuration["Jwt:ExpirationHours"]!);
    }

    public string GenerarToken(string mail, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, mail)
        };

        foreach (var rol in roles)
            claims.Add(new Claim(ClaimTypes.Role, rol));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
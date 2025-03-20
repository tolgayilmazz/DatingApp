using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API;
using API.Entities;
using API.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;


public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly DataContext _context;

    public TokenService(IConfiguration config, DataContext context)
    {
        _config = config;
        _context = context;
    }

    public async Task<string> CreateToken(AppUser user)
    {
        var tokenKey = _config["TokenKey"] ?? throw new Exception("Cannot access tokenKey from appsettings");
        if (tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role ?? "User")
        };

        if (user.Role == "Admin")
        {
            var admin = await _context.Admins
                .Include(a => a.AdminClubs)
                .ThenInclude(ac => ac.Club)
                .FirstOrDefaultAsync(a => a.UserID == user.Id);
            if (admin != null && admin.AdminClubs != null)
            {
                var clubIds = admin.AdminClubs.Select(ac => ac.ClubId).ToList();
                var clubIdsJson = JsonSerializer.Serialize(clubIds, new JsonSerializerOptions { WriteIndented = false });
                claims.Add(new Claim("ClubIds", clubIdsJson));
            }
        }

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = creds

        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
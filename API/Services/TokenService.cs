using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API;
using API.Entities;
using Microsoft.IdentityModel.Tokens;

public class TokenService: ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(AppUser user)
    {
        var tokenKey = _config["TokenKey"] ?? throw new Exception("Cannot access tokenKey from appsettings");
        if (tokenKey.Length < 64) throw new Exception ("Your tokenKey needs to be longer");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.UserName ?? "UnknownUser"),
            new(ClaimTypes.Role, user.Role ?? "User")
        };

        if(user.ClubId.HasValue && user.Role == "Admin"){
            claims.Add(new Claim("ClubId", user.ClubId.Value.ToString()));
        }

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = creds

        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string CreateRefreshToken(){
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create()){
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
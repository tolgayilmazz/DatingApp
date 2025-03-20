using System.ComponentModel.DataAnnotations;
namespace API;

public class TokenRefreshDto
{
    [Required]

    public required string RefreshToken { get; set; }
}
namespace API.DTOs;

public class AddClubDto
{
    public required string ClubName { get; set; }
    public string? LogoUrl { get; set; }
    public required string Description { get; set; }
}
namespace API.DTOs;

public class UpdateAdminDto
{
    public int AdminId { get; set; }
    public List<int> NewClubIds { get; set; } = new List<int>();
}
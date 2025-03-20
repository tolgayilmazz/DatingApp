namespace API.Entities;

public class AdminClub
{
    public int AdminClubId { get; set; }

    public int AdminId { get; set; }
    public required Admin Admin { get; set; } = null!;

    public int ClubId { get; set; }
    public required Club Club { get; set; } = null!;
}
namespace API.Entities;

public class EventLike{
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
namespace API.Entities;

public class EventTicket
{
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
}
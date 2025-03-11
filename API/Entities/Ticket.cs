namespace API.Entities;

public class Ticket{
    public int TicketId { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public required string ReceiptPhoto { get; set; }
    public bool Approved { get; set; } = false;

    public ICollection<EventTicket> EventTickets { get; set; } = new List<EventTicket>();
}
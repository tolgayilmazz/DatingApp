namespace API.DTOs;

public class UserTicketDto 
{
    public int TicketId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string ReceiptPhoto { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public bool Rejected { get; set; }
}
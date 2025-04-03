namespace API.DTOs;

public class ClubTicketDto 
{
    public int TicketId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string ReceiptPhoto { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public bool Rejected { get; set; }
}
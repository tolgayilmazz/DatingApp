using System;
namespace API.DTOs;

public class EventDto
{
    public required string Photo { get; set; }
    public required string EventName { get; set; }
    public required string EventDescription { get; set; }
    public required DateTime EventDate { get; set; }
    public List<int> ClubIds { get; set; } = new();
}
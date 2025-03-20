using System;
using System.Collections.Generic;

namespace API.Entities;


public class Club
{
    public int ClubId { get; set; }
    public required string ClubName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public required string Description { get; set; } = string.Empty;
    public ICollection<AdminClub> AdminClubs { get; set; } = new List<AdminClub>();
    public ICollection<EventClub> EventClubs { get; set; } = new List<EventClub>();

}
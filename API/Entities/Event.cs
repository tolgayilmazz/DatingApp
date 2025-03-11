using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;


public class Event{
    public int EventId {get;set;}
    public required string Photo {get;set;}
    public required string EventName {get;set;}
    public required string EventDescription {get;set;}
    public required DateTime EventDate {get;set;}

    public ICollection<EventClub> EventClubs {get;set;} = new List<EventClub>();
    public ICollection<EventLike> EventLikes {get;set;} = new List<EventLike>();
    public ICollection<EventTicket> EventTickets {get;set;} = new List<EventTicket>();
}
/*using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Event{

    [Key]
    public required int EventId {get;set;}
    [required]
    public required string EventName {get;set;}
    [required]
    public required DateTime EventDate {get;set;}
    [required]
    public required string Location {get;set;}
    [required]
    [Column(TypeName = "decimal(10, 2)")]
    public required decimal Price {get;set;}
    public string ImageUrl {get;set;}
    public int Likes {get; set;} = 0;
    [required]
    [ForeignKey("Club")]
    public int ClubId {get; set;}
    public Club Club {get;set;}

    public required string Description {get;set;}

}*/
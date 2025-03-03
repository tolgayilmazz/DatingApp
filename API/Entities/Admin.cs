using System;
using System.Collections.Generic;


namespace API.Entities;

public class Admin{
    public int AdminId {get;set;}

    public int UserID{get;set;}
    public required AppUser User {get;set;}

    public string Name => User.Name;
    public string Surname => User.Surname;
    public string Email => User.Email;

    public ICollection<AdminClub> AdminClubs {get;set;} = new List<AdminClub>();
}
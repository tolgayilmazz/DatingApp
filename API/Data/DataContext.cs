using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options){

    public DbSet<AppUser> Users { get; set; }
    public DbSet<Admin> Admins {get;set;}
    public DbSet<Club> Clubs {get;set;}
    public DbSet<AdminClub> AdminClubs {get;set;}
    public DbSet<Event> Events {get;set;}
    public DbSet<EventClub> EventClubs {get;set;}
    public DbSet<EventLike> EventLikes {get;set;}
    public DbSet<Ticket> Tickets {get;set;}
    public DbSet<EventTicket> EventTickets {get;set;}
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        //Admin-Club relation
        modelBuilder.Entity<AdminClub>()
            .HasOne(ac => ac.Admin)
            .WithMany(a => a.AdminClubs)
            .HasForeignKey(ac => ac.AdminId);

        modelBuilder.Entity<AdminClub>()
            .HasOne(ac => ac.Club)
            .WithMany(c => c.AdminClubs)
            .HasForeignKey(ac => ac.ClubId);
        

        //Admin-User relation
        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        
        //Events-clubs relation
        modelBuilder.Entity<EventClub>()
            .HasKey(ec => new{ec.EventId, ec.ClubId});
        
        modelBuilder.Entity<EventClub>()
            .HasOne(ec => ec.Event)
            .WithMany(e => e.EventClubs)
            .HasForeignKey(ec => ec.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<EventClub>()
            .HasOne(ec => ec.Club)
            .WithMany(c => c.EventClubs)
            .HasForeignKey(ec => ec.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        
        //Events-Likes relation
        modelBuilder.Entity<EventLike>()
            .HasKey(el => new{el.EventId, el.UserId});
        
        modelBuilder.Entity<EventLike>()
            .HasOne(el => el.Event)
            .WithMany(e => e.EventLikes)
            .HasForeignKey(el => el.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventLike>()
            .HasOne(el => el.User)
            .WithMany(u => u.EventLikes)
            .HasForeignKey(el => el.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        

        //Events-Tickets relation
        modelBuilder.Entity<EventTicket>()
            .HasKey(et => new{et.EventId, et.TicketId});
            
        modelBuilder.Entity<EventTicket>()
            .HasOne(et => et.Event)
            .WithMany(e => e.EventTickets)
            .HasForeignKey(et => et.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<EventTicket>()
            .HasOne(et => et.Ticket)
            .WithMany(t => t.EventTickets)
            .HasForeignKey(et =>et.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
        

        //User-Ticket relation
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
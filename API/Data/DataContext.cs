using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options){

    public DbSet<AppUser> Users { get; set; }
    public DbSet<Admin> Admins {get;set;}
    public DbSet<Club> Clubs {get;set;}
    public DbSet<AdminClub> AdminClubs {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdminClub>()
            .HasOne(ac => ac.Admin)
            .WithMany(a => a.AdminClubs)
            .HasForeignKey(ac => ac.AdminId);

        modelBuilder.Entity<AdminClub>()
            .HasOne(ac => ac.Club)
            .WithMany(c => c.AdminClubs)
            .HasForeignKey(ac => ac.ClubId);

        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
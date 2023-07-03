using Microsoft.EntityFrameworkCore;


namespace RideShare.Models
{

public class RideSharingDbContext : DbContext
{
    public RideSharingDbContext(DbContextOptions<RideSharingDbContext> options) : base(options)
    {
    }

    public DbSet<Ride> Rides { get; set; }
    public DbSet<Commuter> Commuters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships, constraints, and validations
        modelBuilder.Entity<Ride>()
            .HasMany(r => r.Commuters)
            .WithOne(c => c.Ride)
            .HasForeignKey(c => c.RideID);
    }
}
}

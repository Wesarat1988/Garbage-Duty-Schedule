using Garbage_Duty_Schedule.Models;
using Microsoft.EntityFrameworkCore;

namespace Garbage_Duty_Schedule.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Duty> Duties => Set<Duty>();

    public DbSet<Holiday> Holidays => Set<Holiday>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>().HasData(
            new Member { Id = 1, Name = "A", OrderIndex = 1 },
            new Member { Id = 2, Name = "B", OrderIndex = 2 },
            new Member { Id = 3, Name = "C", OrderIndex = 3 }
        );

        modelBuilder.Entity<Member>()
            .HasIndex(m => m.OrderIndex);

        modelBuilder.Entity<Duty>()
            .HasIndex(d => d.Date)
            .IsUnique();
    }
}

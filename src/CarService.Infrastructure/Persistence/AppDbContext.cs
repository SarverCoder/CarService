using CarService.Application.Interfaces;
using CarService.Domain.Common;
using CarService.Domain.Entities;
using CarService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IApplicationDbContext
{
    public DbSet<CarBrand> CarBrands => Set<CarBrand>();
    public DbSet<CarModel> CarModels => Set<CarModel>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<PartCompatibility> PartCompatibilities => Set<PartCompatibility>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AiConversation> AiConversations => Set<AiConversation>();
    public DbSet<AiMessage> AiMessages => Set<AiMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added) entry.Entity.CreatedAt = now;
            else if (entry.State == EntityState.Modified) entry.Entity.UpdatedAt = now;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

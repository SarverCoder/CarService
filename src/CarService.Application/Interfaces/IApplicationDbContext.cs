using CarService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<CarBrand> CarBrands { get; }
    DbSet<CarModel> CarModels { get; }
    DbSet<Category> Categories { get; }
    DbSet<Part> Parts { get; }
    DbSet<PartCompatibility> PartCompatibilities { get; }
    DbSet<ServiceType> ServiceTypes { get; }
    DbSet<Mechanic> Mechanics { get; }
    DbSet<WorkSchedule> WorkSchedules { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Payment> Payments { get; }
    DbSet<AiConversation> AiConversations { get; }
    DbSet<AiMessage> AiMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

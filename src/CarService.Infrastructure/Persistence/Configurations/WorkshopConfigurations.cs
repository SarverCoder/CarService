using CarService.Domain.Entities;
using CarService.Domain.Enums;
using CarService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> b)
    {
        b.Property(x => x.NameUz).HasMaxLength(150).IsRequired();
        b.Property(x => x.NameRu).HasMaxLength(150).IsRequired();
        b.Property(x => x.Description).HasMaxLength(2000);
    }
}

public class MechanicConfiguration : IEntityTypeConfiguration<Mechanic>
{
    public void Configure(EntityTypeBuilder<Mechanic> b)
    {
        b.Property(x => x.FullName).HasMaxLength(150).IsRequired();
        b.Property(x => x.Specialization).HasMaxLength(150);
        b.HasMany(x => x.Schedule).WithOne().HasForeignKey(x => x.MechanicId);
    }
}

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> b)
    {
        b.Property(x => x.VinCode)
            .HasConversion(v => v == null ? null : v.Value, v => v == null ? null : VinCode.Create(v))
            .HasMaxLength(17);
        b.Property(x => x.PlateNumber).HasMaxLength(20);
        b.HasOne(x => x.CarModel).WithMany().HasForeignKey(x => x.CarModelId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.UserId);
    }
}

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> b)
    {
        b.Property(x => x.ContactPhone)
            .HasConversion(v => v.Value, v => PhoneNumber.Create(v))
            .HasMaxLength(16)
            .IsRequired();
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Note).HasMaxLength(1000);
        b.HasOne(x => x.Vehicle).WithMany().HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Mechanic).WithMany().HasForeignKey(x => x.MechanicId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.UserId);

        // Bitta usta bir vaqtda ikkita faol yozuv ololmaydi.
        b.HasIndex(x => new { x.MechanicId, x.StartAt })
            .IsUnique()
            .HasFilter($"\"Status\" <> '{nameof(BookingStatus.Cancelled)}'");
    }
}

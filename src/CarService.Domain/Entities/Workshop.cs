using CarService.Domain.Common;
using CarService.Domain.Enums;
using CarService.Domain.Exceptions;
using CarService.Domain.ValueObjects;

namespace CarService.Domain.Entities;

public class ServiceType : BaseEntity
{
    public string NameUz { get; set; } = null!;
    public string NameRu { get; set; } = null!;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal PriceFrom { get; set; }
    public decimal PriceTo { get; set; }
}

public class Mechanic : BaseEntity
{
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Specialization { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<WorkSchedule> Schedule { get; set; } = [];
}

public class WorkSchedule : BaseEntity
{
    public Guid MechanicId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

public class Vehicle : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CarModelId { get; set; }
    public CarModel CarModel { get; set; } = null!;
    public int Year { get; set; }
    public VinCode? VinCode { get; set; }
    public string? PlateNumber { get; set; }
    public int? Mileage { get; set; }
}

public class Booking : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public Guid ServiceTypeId { get; set; }
    public ServiceType ServiceType { get; set; } = null!;
    public Guid MechanicId { get; set; }
    public Mechanic Mechanic { get; set; } = null!;
    public PhoneNumber ContactPhone { get; set; } = null!;
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? Note { get; set; }

    public void Schedule(DateTime startAt, DateTime endAt)
    {
        if (endAt <= startAt)
            throw new DomainException("Yozuv tugash vaqti boshlanish vaqtidan keyin bo'lishi kerak.");
        StartAt = startAt;
        EndAt = endAt;
    }
}

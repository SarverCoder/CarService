using CarService.Application.Features.Services.Queries;
using CarService.Domain.Entities;

namespace CarService.Application.Tests;

public class GetServicesQueryTests
{
    [Fact]
    public async Task Returns_services_sorted_by_uzbek_name()
    {
        await using var db = TestDbContext.Create();
        db.ServiceTypes.AddRange(
            new ServiceType { NameUz = "Moy almashtirish", NameRu = "Замена масла", DurationMinutes = 30, PriceFrom = 50_000, PriceTo = 100_000 },
            new ServiceType { NameUz = "Diagnostika", NameRu = "Диагностика", DurationMinutes = 45, PriceFrom = 100_000, PriceTo = 150_000 });
        await db.SaveChangesAsync();

        var result = await new GetServicesQueryHandler(db).Handle(new GetServicesQuery(), CancellationToken.None);

        Assert.Equal(["Diagnostika", "Moy almashtirish"], result.Select(s => s.NameUz));
    }
}

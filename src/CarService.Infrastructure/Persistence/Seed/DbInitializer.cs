using CarService.Domain.Entities;
using CarService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarService.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    /// <summary>Migratsiyalarni qo'llaydi, rollar va namunaviy xizmatlarni qo'shadi.</summary>
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        if (!await db.ServiceTypes.AnyAsync())
        {
            db.ServiceTypes.AddRange(
                new ServiceType { NameUz = "Moy almashtirish", NameRu = "Замена масла", DurationMinutes = 30, PriceFrom = 50_000, PriceTo = 100_000 },
                new ServiceType { NameUz = "Kompyuter diagnostikasi", NameRu = "Компьютерная диагностика", DurationMinutes = 45, PriceFrom = 100_000, PriceTo = 150_000 },
                new ServiceType { NameUz = "Tormoz kolodkasini almashtirish", NameRu = "Замена тормозных колодок", DurationMinutes = 60, PriceFrom = 80_000, PriceTo = 150_000 });
            await db.SaveChangesAsync();
        }
    }
}

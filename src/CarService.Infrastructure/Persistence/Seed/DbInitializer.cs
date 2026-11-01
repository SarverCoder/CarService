using CarService.Domain.Entities;
using CarService.Domain.ValueObjects;
using CarService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        await CatalogSeed.SeedAsync(db);

        // Faqat sozlamada berilsa (Development'da) admin foydalanuvchi yaratiladi.
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var adminPhone = config["Seed:Admin:Phone"];
        var adminPassword = config["Seed:Admin:Password"];
        if (!string.IsNullOrWhiteSpace(adminPhone) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var phone = PhoneNumber.Create(adminPhone).Value;
            if (await userManager.FindByNameAsync(phone) is null)
            {
                var admin = new ApplicationUser { UserName = phone, PhoneNumber = phone, FullName = "Administrator" };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }
}
